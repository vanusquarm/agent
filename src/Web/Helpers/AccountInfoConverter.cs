using Statement.Web.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Statement.Web.Helpers
{
    public class AccountInfoConverter : JsonConverter<AccountInfo>
    {
        public override AccountInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var account = new AccountInfo();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return account;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName?.ToLower())
                    {
                        case "accountnumber":
                            account.AccountNumber = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                            break;
                        case "accountname":
                            account.AccountName = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                            break;
                        case "balance":
                            if (reader.TokenType == JsonTokenType.String)
                            {
                                var stringValue = reader.GetString();
                                if (decimal.TryParse(stringValue, out decimal result))
                                    account.Balance = result;
                                else
                                    account.Balance = 0;
                            }
                            else if (reader.TokenType == JsonTokenType.Number)
                            {
                                account.Balance = reader.GetDecimal();
                            }
                            else
                            {
                                account.Balance = 0;
                            }
                            break;
                        case "currency":
                            account.Currency = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                            break;
                        case "branchcode":
                            if (reader.TokenType == JsonTokenType.String)
                            {
                                var stringValue = reader.GetString();
                                if (int.TryParse(stringValue, out int result))
                                    account.BranchCode = result;
                                else
                                    account.BranchCode = 0;
                            }
                            else if (reader.TokenType == JsonTokenType.Number)
                            {
                                account.BranchCode = reader.GetInt32();
                            }
                            else
                            {
                                account.BranchCode = 0;
                            }
                            break;
                        //case "producttype":
                        //    account.ProductType = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                        //    break;
                        case "productcode":
                            account.ProductCode = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                            break;
                        case "email":
                            account.Email = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                            break;
                    }
                }
            }

            return account;
        }

        public override void Write(Utf8JsonWriter writer, AccountInfo value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("AccountNumber", value.AccountNumber);
            writer.WriteString("AccountName", value.AccountName);
            writer.WriteNumber("Balance", value.Balance);
            writer.WriteString("Currency", value.Currency);
            writer.WriteNumber("BranchCode", value.BranchCode);
            //writer.WriteString("ProductType", value.ProductType);
            writer.WriteString("ProductCode", value.ProductCode);
            writer.WriteString("Email", value.Email);
            writer.WriteEndObject();
        }
    }
}