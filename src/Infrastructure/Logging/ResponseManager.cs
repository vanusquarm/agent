using Newtonsoft.Json;
using System.Net;
using System.Xml;

namespace Finacle.Infrastructure.Logging
{
    public static class ResponseManager
    {
        readonly public static Tuple<string, string> Success = new Tuple<string, string>("00", "Success");
        readonly public static Tuple<string, string> FailedTransaction = new Tuple<string, string>("100", "Unable to complete transaction, please try again");
        readonly public static Tuple<string, string> DuplicateRecord = new Tuple<string, string>("94", "Duplicate Record");
        readonly public static Tuple<string, string> FailedValidation = new Tuple<string, string>("30", "One or more validations failed");
        readonly public static Tuple<string, string> InsufficientBalance = new Tuple<string, string>("51", "Insufficient Balance");
        readonly public static Tuple<string, string> SuspectedFraud = new Tuple<string, string>("34", "Suspected fraud");
        readonly public static Tuple<string, string> Exception = new Tuple<string, string>("96", "System malfunction");
        readonly public static Tuple<string, string> PostingError = new Tuple<string, string>("06", "Posting Error");
        readonly public static Tuple<string, string> SessionExpired = new Tuple<string, string>("76", "Reserved for private use");
        readonly public static Tuple<string, string> NoRecordReturned = new Tuple<string, string>("25", "Unable to locate record on file");
        readonly public static Tuple<string, string> Unauthorized = new Tuple<string, string>("05", "Do not honour");
        readonly public static Tuple<string, string> WebServiceReturnedError = new Tuple<string, string>("99", "Error connecting to service");
        readonly public static Tuple<string, string> ReversedTransaction = new Tuple<string, string>("95", "Reconcile error. Transaction Failed and was reversed.");
        readonly public static Tuple<string, string> ValidateTransactionTokenNotEnough = new Tuple<string, string>("55", "Token validation not successful. USSD Tokens have a limit of N.");
        readonly public static Tuple<string, string> ValidateTransactionTokenExceedLimit = new Tuple<string, string>("78", "Token validation not successful. USSD Tokens have a maximum cumulative limit of N.");
        readonly public static Tuple<string, string> ValidateTransactionTokenFailed = new Tuple<string, string>("55", "Token validation not successful. Please generate a new transaction code and try again.");
        readonly public static Tuple<string, string> FailedToGetAmount = new Tuple<string, string>("55", "An error occured validating token. Please try again.");
        readonly public static Tuple<string, string> UsedToken = new Tuple<string, string>("55", "You entered already used code. Please generate a new transaction code and try again.");
        readonly public static Tuple<string, string> IncompleteToken = new Tuple<string, string>("55", "You did not enter the complete code. Please generate a new transaction code and try again.");
        readonly public static Tuple<string, string> TokenTooLong = new Tuple<string, string>("78", "You entered too a long code. Please generate a new transaction code and try again.");
        readonly public static Tuple<string, string> InvalidToken = new Tuple<string, string>("55", "You entered some invalid character in the code. Please generate a new transaction code and try again.");
        readonly public static Tuple<string, string> LockedToken = new Tuple<string, string>("55", "Your token has been locked due to several failed attempts, invalid transaction code or wrong token used. Please retry after 3 minutes  or dial *737*7# on your registered phone number to generate a new token code.");
        readonly public static Tuple<string, string> InvalidTransactionCode = new Tuple<string, string>("55", "Invalid transaction code or wrong token used. Please retry after 3 minutes or dial *737*7# on your registered phone number to generate a new token code.");
        readonly public static Tuple<string, string> FeatureUnavailable = new Tuple<string, string>("40", "This feature is unavailable for the specified request");
        readonly public static Tuple<string, string> SameAccountTransferNotAllowed = new Tuple<string, string>("01", "Cannot transfer between same account number.");
        readonly public static Tuple<string, string> FinacleFailure = new Tuple<string, string>("97", "Unexpected Response from Finacle Data Source");
        readonly public static Tuple<string, string> TokenNotFound = new Tuple<string, string>("01", "Invalid Token Serial");
        readonly public static Tuple<string, string> WrongToken = new Tuple<string, string>("01", "Invalid transaction code or wrong token used. Please retry after 3 minutes or dial *737*7# on your registered phone number to generate a new token code.");
        readonly public static Tuple<string, string> IdentificationError = new Tuple<string, string>("01", "Identification Error Threshold Reached");
        readonly public static Tuple<string, string> DuplicateRequestId = new Tuple<string, string>("94", "Duplicate RequestId");
        readonly public static Tuple<string, string> VirtualAccountDebitNotAllowed = new Tuple<string, string>("01", "Cannot Debit a virtual account");
        readonly public static Tuple<string, string, string> UnAuthorizedAuthMode = new Tuple<string, string, string>("05", "Authentication Mode or Value is required", "UnAuthorized Transaction Because The Auth details are invalid/Incorect. Please, pass correct AuthMode and AuthValue");


        /// <summary>
        /// Formats the response for a Success API call 
        /// </summary>
        /// <param name="Data">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        //public static ResponseWithStatusCode ReturnSuccess(object Data, string? Message = null)
        //{
        //    return new ResponseWithStatusCode()
        //    {
        //        StatusCode = (int)HttpStatusCode.OK,
        //        Response = new GenericBaseResponse()
        //        {
        //            IsSuccess = true,
        //            ResponseCode = Success.Item1,
        //            ResponseDescription = Success.Item2,
        //            Data = Data
        //        }
        //    };
        //}

        /// <summary>
        /// Formats the response for an unsuccessful Finacle API call
        /// </summary>
        /// <param name="exception">the message to return in the Data of the API response.</param>
        /// <param name="message">the message to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        public static CustomProblemDetails ReturnFinacleCallUnsuccessful(dynamic? exception = null, dynamic? FinacleData = null, string? message = null)
        {
            return new CustomProblemDetails()
            {
                ResponseCode = FinacleFailure.Item1,
                IsSuccess = false,
                Title = message ?? FinacleFailure.Item2,
                Status = (int)HttpStatusCode.UnprocessableEntity,
                Extensions = new Dictionary<string, object?> { { "errors", exception } },
                FinacleData = FinacleData
            };
        }

        /// <summary>
        /// Formats the response for a "Feature Unavailable" API call 
        /// </summary>
        /// <param name="message">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        //public static ResponseWithStatusCode ReturnFeatureUnavailable(string message = "")
        //{
        //    return new ResponseWithStatusCode()
        //    {
        //        StatusCode = (int)HttpStatusCode.UnprocessableEntity,
        //        Response = new CustomProblemDetails()
        //        {
        //            ResponseCode = FeatureUnavailable.Item1,
        //            IsSuccess = false,
        //            Title = FeatureUnavailable.Item2,
        //            Status = (int)HttpStatusCode.UnprocessableEntity,
        //            Extensions = new Dictionary<string, object?> { { "errors", message } }
        //        }
        //    };
        //}

        /// <summary>
        /// Formats the response for an Exception API call 
        /// </summary>
        /// <param name="exception">the message to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        //public static ResponseWithStatusCode ReturnException(dynamic? exception = null)
        //{
        //    return new ResponseWithStatusCode()
        //    {
        //        StatusCode = (int)HttpStatusCode.InternalServerError,
        //        Response = new CustomProblemDetails()
        //        {
        //            ResponseCode = Exception.Item1,
        //            IsSuccess = false,
        //            Title = Exception.Item2,
        //            Status = (int)HttpStatusCode.InternalServerError,
        //            Extensions = new Dictionary<string, object?> { { "errors", exception } }
        //        }
        //    };
        //}

        /// <summary>
        /// Formats the response for a FailedValidations API call 
        /// </summary>
        /// <param name="validationErrors">the serialized validation errors to return.</param>
        /// <returns>ApiResponse</returns>
        public static CustomProblemDetails ReturnFailedValidations(string validationErrors)
        {
            return new CustomProblemDetails()
            {
                ResponseCode = FailedValidation.Item1,
                IsSuccess = false,
                Title = FailedValidation.Item2,
                Status = (int)HttpStatusCode.BadRequest,
                Extensions = new Dictionary<string, object?> { { "errors", validationErrors } }
            };
        }

        /// <summary>
        /// Formats the response for an Unauthrized entity API call 
        /// </summary>
        /// <param name="errors">the serialized errors to return.</param>
        /// <returns>ApiResponse</returns>
        //public static ResponseWithStatusCode ReturnUnauthorized(string message = "Invalid or Incorrect Credentials")
        //{
        //    return new ResponseWithStatusCode()
        //    {
        //        StatusCode = (int)HttpStatusCode.Unauthorized,
        //        Response = new CustomProblemDetails()
        //        {
        //            ResponseCode = Unauthorized.Item1,
        //            IsSuccess = false,
        //            Title = Unauthorized.Item2,
        //            Detail = message,
        //            Status = (int)HttpStatusCode.Unauthorized,
        //        }
        //    };
        //}

        /// <summary>
        /// Formats the response for an Unauthrized entity API call 
        /// </summary>
        /// <param name="errors">the serialized errors to return.</param>
        /// <returns>ApiResponse</returns>
        public static CustomProblemDetails ReturnNullAuthModeUnauthorized(string? message = null)
        {
            return new CustomProblemDetails()
            {
                ResponseCode = UnAuthorizedAuthMode.Item1,
                IsSuccess = false,
                Title = UnAuthorizedAuthMode.Item2,
                Detail = message ?? UnAuthorizedAuthMode.Item2,
                Status = (int)HttpStatusCode.Unauthorized,
            };
        }

        /// <summary>
        /// Formats the response for a FailedValidations API call 
        /// </summary>
        /// <param name="validationErrors">the serialized validation errors to return.</param>
        /// <returns>ApiResponse</returns>
        public static CustomProblemDetails ReturnFailedValidationsV2(string? validationErrors)
        {
            return new CustomProblemDetails()
            {
                ResponseCode = FailedValidation.Item1,
                IsSuccess = false,
                Title = FailedValidation.Item2,
                Detail = validationErrors,
                Status = (int)HttpStatusCode.BadRequest,
                Extensions = new Dictionary<string, object?> { { "errors", validationErrors } }
            };
        }

        /// <summary>
        /// Formats the response for an Unauthrized entity API call 
        /// </summary>
        /// <param name="errors">the serialized errors to return.</param>
        /// <returns>ApiResponse</returns>
        public static CustomProblemDetails ReturnAuthModeUnauthorized()
        {
            return new CustomProblemDetails()
            {
                ResponseCode = UnAuthorizedAuthMode.Item1,
                IsSuccess = false,
                Title = UnAuthorizedAuthMode.Item2,
                Detail = UnAuthorizedAuthMode.Item3,
                Status = (int)HttpStatusCode.Unauthorized,
            };
        }

        /// <summary>
        /// Formats the response for a Required Session to Transfer API call 
        /// </summary>
        /// <param name="Data">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        //public static ResponseWithStatusCode ReturnFraudResponse(string Message)
        //{
        //    return new ResponseWithStatusCode()
        //    {
        //        StatusCode = (int)HttpStatusCode.Forbidden,
        //        Response = new CustomProblemDetails()
        //        {
        //            ResponseCode = SuspectedFraud.Item1,
        //            IsSuccess = false,
        //            Title = SuspectedFraud.Item2 + " | " + "Fraud Detected",
        //            Detail = Message,
        //            Status = (int)HttpStatusCode.Forbidden,
        //        }
        //    };
        //}

        /// <summary>
        /// Formats the response for a gtworld duplicate record to Transfer API call 
        /// </summary>
        /// <param name="Data">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        //public static ResponseWithStatusCode ReturnGTWorldDuplicateRecord(string Message)
        //{
        //    return new ResponseWithStatusCode()
        //    {
        //        StatusCode = (int)HttpStatusCode.Ambiguous,
        //        Response = new CustomProblemDetails()
        //        {
        //            ResponseCode = DuplicateRecord.Item1,
        //            IsSuccess = false,
        //            Title = DuplicateRecord.Item2,
        //            Detail = Message,
        //            Status = (int)HttpStatusCode.Ambiguous
        //        }
        //    };
        //}


        /// <summary>
        /// Formats the response for a duplicate Call to Transfer API call 
        /// </summary>
        /// <param name="Data">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        //public static ResponseWithStatusCode ReturnFinacleDuplicateRecord(string Message, Object FinacleData)
        //{
        //    return new ResponseWithStatusCode()
        //    {
        //        StatusCode = (int)HttpStatusCode.UnprocessableEntity,
        //        Response = new CustomProblemDetails()
        //        {
        //            ResponseCode = DuplicateRecord.Item1,
        //            IsSuccess = false,
        //            Title = DuplicateRecord.Item2,
        //            Detail = Message,
        //            Status = (int)HttpStatusCode.UnprocessableEntity,
        //            FinacleData = FinacleData != null ? JsonConvert.SerializeObject(FinacleData) : null
        //        }
        //    };
        //}
        /// <summary>
        /// Formats the response for Generic Failed Vlidation to Transfer API call 
        /// </summary>
        /// <param name="Data">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        //public static ResponseWithStatusCode ReturnFailed(string Message)
        //{
        //    return new ResponseWithStatusCode()
        //    {
        //        StatusCode = (int)HttpStatusCode.UnprocessableEntity,
        //        Response = new CustomProblemDetails()
        //        {
        //            ResponseCode = FailedTransaction.Item1,
        //            IsSuccess = false,
        //            Title = FailedTransaction.Item2,
        //            Detail = Message,
        //            Status = (int)HttpStatusCode.UnprocessableEntity
        //        }
        //    };
        //}

        /// <summary>
        /// Formats the response for No record Found to Transfer API call 
        /// </summary>
        /// <param name="Data">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        //public static ResponseWithStatusCode ReturnNoRecordFound(string Message)
        //{
        //    return new ResponseWithStatusCode()
        //    {
        //        StatusCode = (int)HttpStatusCode.UnprocessableEntity,
        //        Response = new CustomProblemDetails()
        //        {
        //            ResponseCode = NoRecordReturned.Item1,
        //            IsSuccess = false,
        //            Title = NoRecordReturned.Item2,
        //            Detail = Message,
        //            Status = (int)HttpStatusCode.UnprocessableEntity
        //        }
        //    };
        //}

        /// <summary>
        /// Formats the response for Insufficient Balance to Transfer API call 
        /// </summary>
        /// <param name="Data">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        //public static ResponseWithStatusCode ReturnInsufficientBalance()
        //{
        //    return new ResponseWithStatusCode()
        //    {
        //        StatusCode = (int)HttpStatusCode.UnprocessableEntity,
        //        Response = new CustomProblemDetails()
        //        {
        //            ResponseCode = InsufficientBalance.Item1,
        //            IsSuccess = false,
        //            Title = InsufficientBalance.Item2,
        //            Detail = InsufficientBalance.Item2,
        //            Status = (int)HttpStatusCode.UnprocessableEntity
        //        }
        //    };
        //}

        /// <summary>
        /// Returns a response for unexpected Finacle Responses like 500
        /// </summary>
        /// <param name="exception">the message to return in the Data of the API response.</param>
        /// <param name="message">the message to return in the Data of the API response.</param>
        /// <returns>ResponseWithStatusCode</returns>
        //public static ResponseWithStatusCode ReturnFinacleFailure(dynamic? exception = null, string? message = null)
        //{
        //    return new ResponseWithStatusCode()
        //    {
        //        StatusCode = (int)HttpStatusCode.UnprocessableEntity,
        //        Response = new CustomProblemDetails()
        //        {
        //            ResponseCode = FinacleFailure.Item1,
        //            IsSuccess = false,
        //            Title = message ?? FinacleFailure.Item2,
        //            Status = (int)HttpStatusCode.UnprocessableEntity,
        //            Extensions = new Dictionary<string, object?> { { "errors", exception } }
        //        }
        //    };
        //}

        /// <summary>
        /// Formats the response for a No Record Found API call 
        /// </summary>
        /// <param name="message">A Customized message to send </param>
        /// <returns>ResponseWithStatusCode</returns>
        //public static ResponseWithStatusCode ReturnNoRecordsFound(string? message = null)
        //{
        //    return new ResponseWithStatusCode()
        //    {
        //        StatusCode = (int)HttpStatusCode.UnprocessableEntity,
        //        Response = new CustomProblemDetails()
        //        {
        //            ResponseCode = NoRecordReturned.Item1,
        //            IsSuccess = false,
        //            Title = NoRecordReturned.Item2,
        //            Status = (int)HttpStatusCode.UnprocessableEntity,
        //            Extensions = new Dictionary<string, object?> { { "errors", message ?? NoRecordReturned.Item2 } }
        //        }
        //    };
        //}


        //public static TokenValidationDto ValidateTokenWithAmountResponse(string tokenValidateRes)
        //{
        //    var xmlDoc = new XmlDocument();
        //    xmlDoc.LoadXml(tokenValidateRes);
        //    var xData = JsonConvert.SerializeXmlNode(xmlDoc);
        //    var data = JsonConvert.DeserializeObject<TokenValidationDto>(xData);
        //    return data;

        //}

        /// <summary>
        /// Formats the response for a Success API call 
        /// </summary>
        /// <param name="Data">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        public static GenericBaseResponse ReturnSuccess(object Data, string? Message = null)
        {

            return new GenericBaseResponse()
            {
                IsSuccess = true,
                ResponseCode = Success.Item1,
                ResponseDescription = Success.Item2,
                Data = Data
            };

        }

        /// <summary>
        /// Formats the response for an unsuccessful Finacle API call
        /// </summary>
        /// <param name="exception">the message to return in the Data of the API response.</param>
        /// <param name="message">the message to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        public static GenericBaseResponse ReturnFinacleCallUnsuccessful(object Data, string? Message = null)
        {

            return new GenericBaseResponse()
            {
                ResponseCode = FinacleFailure.Item1,
                IsSuccess = false,
                ResponseDescription = Message ?? FinacleFailure.Item2,
                Data = Data
            };

        }

        /// <summary>
        /// Formats the response for a "Feature Unavailable" API call 
        /// </summary>
        /// <param name="message">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        public static GenericBaseResponse ReturnFeatureUnavailable(object Data, string? Message = null)
        {

            return new GenericBaseResponse()
            {
                ResponseCode = FeatureUnavailable.Item1,
                IsSuccess = false,
                ResponseDescription = Message ?? FinacleFailure.Item2,
                Data = Data
            };

        }

        /// <summary>
        /// Formats the response for an Exception API call 
        /// </summary>
        /// <param name="exception">the message to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        public static GenericBaseResponse ReturnException(object Data, string? Message = null)
        {

            return new GenericBaseResponse()
            {
                ResponseCode = Exception.Item1,
                IsSuccess = false,
                ResponseDescription = Message ?? Exception.Item2,
                Data = Data
            };

        }

        /// <summary>
        /// Formats the response for a FailedValidations API call 
        /// </summary>
        /// <param name="validationErrors">the serialized validation errors to return.</param>
        /// <returns>ApiResponse</returns>
        public static GenericBaseResponse ReturnFailedValidations(object? Data, string? Message = null)
        {

            return new GenericBaseResponse()
            {
                ResponseCode = FailedValidation.Item1,
                IsSuccess = false,
                ResponseDescription = Message ?? FailedValidation.Item2,
                Data = Data,
            };

        }

        /// <summary>
        /// Formats the response for an Unauthrized entity API call 
        /// </summary>
        /// <param name="errors">the serialized errors to return.</param>
        /// <returns>ApiResponse</returns>
        public static GenericBaseResponse ReturnUnauthorized(object Data, string Message = "Invalid or Incorrect Credentials")
        {

            return new GenericBaseResponse()
            {
                ResponseCode = Unauthorized.Item1,
                IsSuccess = false,
                ResponseDescription = Message ?? Unauthorized.Item2,
                Data = Data
            };

        }


        /// <summary>
        /// Formats the response for a Required Session to Transfer API call 
        /// </summary>
        /// <param name="Data">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        public static GenericBaseResponse ReturnFraudResponse(object Data, string? Message)
        {

            return new GenericBaseResponse
            {
                ResponseCode = SuspectedFraud.Item1,
                IsSuccess = false,
                ResponseDescription = (Message ?? SuspectedFraud.Item2) + " | " + "Fraud Detected",
                Data = Data
            };
        }



        /// <summary>
        /// Formats the response for a duplicate Call to Transfer API call 
        /// </summary>
        /// <param name="Data">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        public static GenericBaseResponse ReturnFinacleDuplicateRecord(object? Data, string? Message)
        {

            return new GenericBaseResponse()
            {
                ResponseCode = DuplicateRecord.Item1,
                IsSuccess = false,
                ResponseDescription = Message ?? DuplicateRecord.Item2,
                Data = Data
            };

        }
        /// <summary>
        /// Formats the response for Generic Failed Vlidation to Transfer API call 
        /// </summary>
        /// <param name="Data">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        public static GenericBaseResponse ReturnFailed(object? Data, string? Message)
        {

            return new GenericBaseResponse()
            {
                ResponseCode = FailedTransaction.Item1,
                IsSuccess = false,
                ResponseDescription = Message ?? FailedTransaction.Item2,
                Data = Data
            };

        }

        /// <summary>
        /// Formats the response for No record Found to Transfer API call 
        /// </summary>
        /// <param name="Data">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        public static GenericBaseResponse ReturnNoRecordFound(object Data, string? Message)
        {

            return new GenericBaseResponse()
            {
                ResponseCode = NoRecordReturned.Item1,
                IsSuccess = false,
                ResponseDescription = Message ?? NoRecordReturned.Item2,
                Data = Data
            };

        }

        /// <summary>
        /// Formats the response for Insufficient Balance to Transfer API call 
        /// </summary>
        /// <param name="Data">the object to return in the Data of the API response.</param>
        /// <returns>ApiResponse</returns>
        public static GenericBaseResponse ReturnInsufficientBalance(object Data, string? Message)
        {

            return new GenericBaseResponse()
            {
                ResponseCode = InsufficientBalance.Item1,
                IsSuccess = false,
                ResponseDescription = Message ?? InsufficientBalance.Item2,
                Data = Data
            };

        }

        /// <summary>
        /// Returns a response for unexpected Finacle Responses like 500
        /// </summary>
        /// <param name="exception">the message to return in the Data of the API response.</param>
        /// <param name="message">the message to return in the Data of the API response.</param>
        /// <returns>ResponseWithStatusCode</returns>
        public static GenericBaseResponse ReturnFinacleFailure(object Data, string? Message = null)
        {

            return new GenericBaseResponse()
            {
                ResponseCode = FinacleFailure.Item1,
                IsSuccess = false,
                ResponseDescription = Message ?? FinacleFailure.Item2,
                Data = Data
            };

        }

        /// <summary>
        /// Formats the response for a No Record Found API call 
        /// </summary>
        /// <param name="message">A Customized message to send </param>
        /// <returns>ResponseWithStatusCode</returns>
        public static GenericBaseResponse ReturnNoRecordsFound(object Data, string? Message = null)
        {

            return new GenericBaseResponse()
            {
                ResponseCode = NoRecordReturned.Item1,
                IsSuccess = false,
                ResponseDescription = Message ?? NoRecordReturned.Item2,
                Data = Data
            };

        }


        //public static TokenValidationDto ValidateTokenWithAmountResponse(string tokenValidateRes)
        //{
        //    var xmlDoc = new XmlDocument();
        //    xmlDoc.LoadXml(tokenValidateRes);
        //    var xData = JsonConvert.SerializeXmlNode(xmlDoc);
        //    var data = JsonConvert.DeserializeObject<TokenValidationDto>(xData);
        //    return data;

        //}

        public class ResponseWithStatusCode
        {
            public int StatusCode { get; set; }
            public object? Response { get; set; }
        }

        public class GenericBaseResponse
        {
            public string ResponseCode { get; set; }
            public string ResponseDescription { get; set; }
            public bool IsSuccess { get; set; }
            public object? Data { get; set; }
        }

    }


    public static class FinacleErrorsWarningsAndExceptionManager
    {
        readonly public static Tuple<string, string> DuplicateRequestId = new Tuple<string, string>("E0202", "Unable to insert record in UTT (bufman error = [-1])");
    }

}