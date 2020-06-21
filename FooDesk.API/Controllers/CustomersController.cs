using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FooDesk.Core.Entities;
using FooDesk.Data;
using FooDesk.Core.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNet.OData;
using NSwag.Annotations;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace FooDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly FooDeskContext _context;        
        private readonly IMapper _mapper;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionAccessor;

        public CustomersController(FooDeskContext context, IMapper mapper, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionAccessor)
        {
            _context = context;
            _mapper = mapper;
            _urlHelperFactory = urlHelperFactory;
            _actionAccessor = actionAccessor;
        }

        /// <summary>
        /// Get all customers.
        /// </summary>
        /// <returns>Customer</returns>
        [HttpGet(Name = nameof(GetCustomers))]
        [EnableQuery]
        [ProducesResponseType(200)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<CustomerDto>), Description = "Retrieved all customers.")]
        public IActionResult GetCustomers() => Ok(_mapper.ProjectTo<CustomerDto>(_context.Customers.AsQueryable()));

        /// <summary>
        /// Get a customer.
        /// </summary>
        /// <param name="id">The customer id to retrieve.</param>
        /// <returns>Customer</returns>
        [HttpGet("{id}", Name = nameof(GetCustomer))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CustomerDto), Description = "The customer was succesfully found.")]
        [SwaggerResponse(HttpStatusCode.NotFound, null, Description = "The customer was not found.")]
        public async Task<IActionResult> GetCustomer(int id) => Ok(_mapper.Map<CustomerDto>(await _context.Customers.FindAsync(id)));

        /// <summary>
        /// Partially update a customer.
        /// </summary>
        /// <param name="id">The customer id to update.</param>
        /// <param name="customer">The customer object.</param>
        /// <returns>Customer</returns>
        [HttpPatch("{id}", Name = nameof(PatchCustomer))]      
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CustomerDto), Description = "The customer was succesfully updated.")]
        [SwaggerResponse(HttpStatusCode.NotFound, null, Description = "The customer was not found.")]
        
        public async Task<IActionResult> PatchCustomer(int id, Delta<CustomerDto> customer)
        {
            var target = _context.Customers.Find(id);

            foreach (string propertyName in customer.GetChangedPropertyNames())
            {
                if (customer.TryGetPropertyValue(propertyName, out object propertyValue))
                {
                    var propertyInfo = typeof(Customer).GetProperty(propertyName);
                    propertyInfo.SetValue(target, propertyValue);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(target);
        }

        /// <summary>
        /// Update a customer.
        /// </summary>
        /// <param name="id">The customer id to update.</param>
        /// <param name="customer">The customer object.</param>
        /// <returns>Customer</returns>
        [HttpPut("{id}", Name = nameof(PutCustomer))]        
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CustomerDto), Description = "The customer was succesfully updated.")]
        [SwaggerResponse(HttpStatusCode.NotFound, null, Description = "The customer was not found.")]
        public async Task<IActionResult> PutCustomer(int id, CustomerDto customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Create a customer.
        /// </summary>
        /// <returns>Customer</returns>
        [HttpPost(Name =nameof(PostCustomer))]
        [ProducesResponseType(200)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CustomerDto), Description = "The customer was succesfully created.")]
        public async Task<IActionResult> PostCustomer(CustomerDto customer)
        {
            var customerToCreate = _mapper.Map<Customer>(customer);
            _context.Customers.Add(customerToCreate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        /// <summary>
        /// Delete a customer.
        /// </summary>
        /// <param name="id">The customer id to delete.</param>
        /// <returns>None</returns>
        [HttpDelete("{id}", Name = nameof(DeleteCustomer))]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [SwaggerResponse(HttpStatusCode.NoContent, null, Description = "The customer was succesfully deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, null, Description = "The customer was not found")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        //private static CustomerDto CreateLinksForCustomer(CustomerDto customer, IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory)
        //{
        //    var urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);

        //    var idObj = new { id = customer.Id };
        //    customer.Links.Add(
        //        new LinkDto(urlHelper.Link(nameof(GetCustomer), idObj),
        //        "self",
        //        "GET"));

        //    customer.Links.Add(
        //        new LinkDto(urlHelper.Link(nameof(PutCustomer), idObj),
        //        "update_user",
        //        "PUT"));

        //    customer.Links.Add(
        //        new LinkDto(urlHelper.Link(nameof(DeleteCustomer), idObj),
        //        "delete_user",
        //        "DELETE"));

        //    return customer;
        //}
    }
}
