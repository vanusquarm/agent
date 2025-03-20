using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Transactions.API.Entities;
using Transactions.API.Services;

namespace Transactions.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class FxTransferController : ControllerBase
    {

        private readonly ITransactionClient _fxSalesTransferService;
        public FxTransferController(ITransactionClient fxSalesTransferService)
        {
            _fxSalesTransferService = fxSalesTransferService;
        }

        /// <summary>
        /// Channel FX Sales Transfer Endpoint. Transfer between currencies
        /// </summary>
        /// <param name="hearderRequest"></param>
        /// <param name="Request"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)] // Specifies that the action can return a string with status code 200
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Specifies that the action can return a status code 400 if the resource is not found
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Specifies that the action can return a status code 401 if the resource is unauthorized
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Specifies that the action can return a status code 500 if the resource is InternalServerError
        [HttpPost, Route("FXSale")]
        public async Task<ActionResult<dynamic>> FxSales([FromBody] FxSalesRequest fxRequest)
        {
            //HttpContext.Items.Add("RequestPayload", JsonSerializer.Serialize(fxRequest));
            //var channelTransferResponse = await _fxSalesTransferService.ActionFxTransfer(fxRequest);
            //return StatusCode(channelTransferResponse.StatusCode, channelTransferResponse.Response);
            return Ok("Not Implemented");
        }

        /// <summary>
        /// Get fx rates using the rate code
        /// </summary>
        /// <param name="hearderRequest"></param>
        /// <param name="Request"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)] // Specifies that the action can return a string with status code 200
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Specifies that the action can return a status code 400 if the resource is not found
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Specifies that the action can return a status code 401 if the resource is unauthorized
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Specifies that the action can return a status code 500 if the resource is InternalServerError
        [HttpPost, Route("Rates")]
        public async Task<ActionResult<dynamic>> Rates([FromBody] RatesRequestModel Request)
        {
            //HttpContext.Items.Add("RequestPayload", JsonSerializer.Serialize(Request));
            //var fxRates = await _fxSalesTransferService.GetFxRates(Request);
            //return StatusCode(fxRates.StatusCode, fxRates.Response);
            return Ok("Not Implemented");
        }

        /// <summary>
        /// This endpoint is responsible for initiating currency conversion between two currencies
        /// </summary>
        /// <param name="request"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)] // Specifies that the action can return a string with status code 200
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Specifies that the action can return a status code 400 if the resource is not found
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Specifies that the action can return a status code 401 if the resource is unauthorized
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Specifies that the action can return a status code 500 if the resource is InternalServerError
        [HttpPost, Route("Convert")]
        public async Task<ActionResult<dynamic>> ConvertCurrency(CurrencyConversionReqVM request)
        {


            //HttpContext.Items.Add("RequestPayload", JsonSerializer.Serialize(request));
            //var response = await _fxSalesTransferService.ConvertCurrency(request);
            //return StatusCode(response.StatusCode, response.Response);
            return Ok("Not Implemented");
        }

    }
}
