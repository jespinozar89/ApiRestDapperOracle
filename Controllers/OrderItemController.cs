using Microsoft.AspNetCore.Mvc;
using MyApiRestDapperOracle.Models.DTOs;
using MyApiRestDapperOracle.Models.Entities;
using MyApiRestDapperOracle.Services.Interfaces;
using Oracle.ManagedDataAccess.Client;

namespace MyApiRestDapperOracle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;
        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var orderItems = await _orderItemService.GetAll();
                return Ok(orderItems);
            }
            catch (OracleException ex) when (ex.Number >= 20000 && ex.Number <= 20999)
            {
                // Errores personalizados de Oracle (RAISE_APPLICATION_ERROR)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("{orderId}/{lineItemId}")]
        public async Task<IActionResult> GetById(int orderId, int lineItemId)
        {
            try
            {
                var orderItem = await _orderItemService.GetById(orderId, lineItemId);
                if (orderItem == null)
                {
                    return NotFound($"Order item with Order ID {orderId} and Line Item ID {lineItemId} not found.");
                }
                return Ok(orderItem);
            }
            catch (OracleException ex) when (ex.Number >= 20000 && ex.Number <= 20999)
            {
                // Errores personalizados de Oracle (RAISE_APPLICATION_ERROR)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] OrderItemDTO orderItem)
        {
            try
            {
                if (orderItem == null)
                {
                    return BadRequest("Order item cannot be null.");
                }

                var newOrderItem = new OrderItem
                {
                    OrderId = orderItem.OrderId,
                    LineItemId = orderItem.LineItemId,
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Quantity,
                    UnitPrice = orderItem.UnitPrice,
                    ShipmentId = 1
                };

                await _orderItemService.Add(newOrderItem);
                return CreatedAtAction(nameof(GetById), new { orderId = newOrderItem.OrderId, lineItemId = newOrderItem.LineItemId }, newOrderItem);
            }
            catch (OracleException ex) when (ex.Number >= 20000 && ex.Number <= 20999)
            {
                // Errores personalizados de Oracle (RAISE_APPLICATION_ERROR)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] OrderItemDTO orderItem)
        {
            try
            {
                if (orderItem == null)
                {
                    return BadRequest("Order item cannot be null.");
                }

                var existingOrderItem = await _orderItemService.GetById(orderItem.OrderId, orderItem.LineItemId);
                if (existingOrderItem == null)
                {
                    return NotFound($"Order item with Order ID {orderItem.OrderId} and Line Item ID {orderItem.LineItemId} not found.");
                }

                existingOrderItem.ProductId = orderItem.ProductId;
                existingOrderItem.Quantity = orderItem.Quantity;
                existingOrderItem.UnitPrice = orderItem.UnitPrice;
                existingOrderItem.ShipmentId = 1;

                await _orderItemService.Update(existingOrderItem);
                return NoContent();
            }
            catch (OracleException ex) when (ex.Number >= 20000 && ex.Number <= 20999)
            {
                // Errores personalizados de Oracle (RAISE_APPLICATION_ERROR)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete("{orderId}/{lineItemId}")]
        public async Task<IActionResult> Delete(int orderId, int lineItemId)
        {
            try
            {
                var existingOrderItem = await _orderItemService.GetById(orderId, lineItemId);
                if (existingOrderItem == null)
                {
                    return NotFound($"Order item with Order ID {orderId} and Line Item ID {lineItemId} not found.");
                }

                await _orderItemService.Delete(orderId, lineItemId);
                return NoContent();
            }
            catch (OracleException ex) when (ex.Number >= 20000 && ex.Number <= 20999)
            {
                // Errores personalizados de Oracle (RAISE_APPLICATION_ERROR)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}