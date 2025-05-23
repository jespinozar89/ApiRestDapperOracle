using Microsoft.AspNetCore.Mvc;
using MyApiRestDapperOracle.Models.DTOs;
using MyApiRestDapperOracle.Models.Entities;
using MyApiRestDapperOracle.Services.Interfaces;
using Oracle.ManagedDataAccess.Client;

namespace MyApiRestDapperOracle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var customer = await _customerService.GetById(id);
                if (customer == null)
                {
                    return NotFound($"Customer with ID {id} not found.");
                }
                return Ok(customer);
            }
            catch (OracleException ex) when (ex.Number >= 20000 && ex.Number <= 20999)
            {
                // Errores personalizados de Oracle (RAISE_APPLICATION_ERROR)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var customers = await _customerService.GetAll();
                return Ok(customers);
            }
            catch (OracleException ex) when (ex.Number >= 20000 && ex.Number <= 20999)
            {
                // Errores personalizados de Oracle (RAISE_APPLICATION_ERROR)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(CustomerDTO customer)
        {
            try
            {
                if (customer == null)
                {
                    return BadRequest("Customer object is null.");
                }

                var newCustomer = new Customer
                {
                    EmailAddress = customer.EmailAddress,
                    FullName = customer.FullName
                };

                newCustomer.CustomerId = await _customerService.Add(newCustomer);
                return CreatedAtAction(nameof(GetById), new { id = newCustomer.CustomerId }, newCustomer);
            }
            catch (OracleException ex) when (ex.Number >= 20000 && ex.Number <= 20999)
            {
                // Errores personalizados de Oracle (RAISE_APPLICATION_ERROR)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(CustomerDTO customer)
        {
            try
            {
                if (customer == null)
                {
                    return BadRequest("Customer object is null.");
                }

                var updateCustomer = new Customer
                {
                    CustomerId = customer.CustomerId,
                    EmailAddress = customer.EmailAddress,
                    FullName = customer.FullName
                };

                await _customerService.Update(updateCustomer);
                return NoContent();
            }
            catch (OracleException ex) when (ex.Number >= 20000 && ex.Number <= 20999)
            {
                // Errores personalizados de Oracle (RAISE_APPLICATION_ERROR)
                return BadRequest(ex.Message);
            }
            catch (OracleException ex) when (ex.Number == 20004)
            {
                throw new KeyNotFoundException($"Cliente con ID {customer.CustomerId} no encontrado", ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _customerService.Delete(id);
                return NoContent();
            }
            catch (OracleException ex) when (ex.Number >= 20000 && ex.Number <= 20999)
            {
                // Errores personalizados de Oracle (RAISE_APPLICATION_ERROR)
                return BadRequest(ex.Message);
            }
            catch (OracleException ex) when (ex.Number == 20001)
            {
                throw new ArgumentException("ID de cliente es requerido", ex);
            }
            catch (OracleException ex) when (ex.Number == 20004)
            {
                throw new KeyNotFoundException($"Cliente con ID {id} no encontrado", ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}