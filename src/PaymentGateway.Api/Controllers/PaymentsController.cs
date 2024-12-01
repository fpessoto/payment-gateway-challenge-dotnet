using Ardalis.Result;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Core.Exceptions;
using PaymentGateway.UseCases.Payments;
using PaymentGateway.UseCases.Payments.Create;
using PaymentGateway.UseCases.Payments.Get;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IMediator mediator) : Controller
{
    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Post([FromBody] CreatePaymentRequest request)
    {
        try
        {
            var result = await mediator.Send(new CreatePaymentCommand(request.CardNumber, request.ExpiryMonth,
                request.ExpiryYear, request.Cvv, request.Currency, request.Amount));

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(string.Join(", ", result.ValidationErrors));
        }
        catch (BusinessException e)
        {
            return BadRequest(new { Status = "Rejected", Message = e.Message });
        }
        catch (ExternalServiceUnavailableException e)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        catch (Exception e)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentDto?>> GetPaymentAsync(Guid id)
    {
        var result = await mediator.Send(new GetPaymentQuery(id));

        if (result.Status == ResultStatus.NotFound)
        {
            return NotFound();
        }

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest();
    }
}