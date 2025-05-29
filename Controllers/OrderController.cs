using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyApiRestDapperOracle.Models.DTOs;
using MyApiRestDapperOracle.Models.Entities;
using MyApiRestDapperOracle.Services.Interfaces;
using Oracle.ManagedDataAccess.Client;

namespace MyApiRestDapperOracle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var orders = await _orderService.GetAll();
                return Ok(orders);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var order = await _orderService.GetById(id);
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }
                return Ok(order);
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
        public async Task<IActionResult> Add(OrderDTO order)
        {
            try
            {
                if (order == null)
                {
                    return BadRequest("Order cannot be null.");
                }

                var newOrder = new Order
                {
                    CustomerId = order.CustomerId,
                    OrderTms = DateTime.Now,
                    OrderStatus = order.OrderStatus,
                    StoreId = 1
                };

                // Llama al servicio para agregar el pedido
                newOrder.OrderId = await _orderService.Add(newOrder);
                return CreatedAtAction(nameof(GetById), new { id = newOrder.OrderId }, newOrder);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderDTO order)
        {
            try
            {
                if (order == null)
                {
                    return BadRequest("Order cannot be null.");
                }

                if (id != order.OrderId)
                {
                    return BadRequest("Error Order ID mismatch. The ID in the URL must match the ID in the order object.");
                }

                var existingOrder = await _orderService.GetById(id);
                if (existingOrder == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                existingOrder.OrderId = id; // Aseguramos que el ID del pedido sea el correcto
                existingOrder.CustomerId = order.CustomerId;
                existingOrder.OrderTms = DateTime.Now; // Actualizamos la fecha y hora del pedido
                existingOrder.OrderStatus = order.OrderStatus;
                existingOrder.StoreId = 1;

                await _orderService.Update(existingOrder);
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
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existingOrder = await _orderService.GetById(id);
                if (existingOrder == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                await _orderService.Delete(id);
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