﻿using System;
using Newtonsoft.Json;
using PhuLongCRM.Models;

namespace PhuLongCRM.Helper
{
	public class Decoder
	{
        public static (JwtHeader Header, string Payload, string Verification) DecodeToken(string token)
        {
            string[] split = token.Split('.');
            if (split.Length > 1)
            {
                JwtHeader jsonHeaderData = JsonConvert.DeserializeObject<JwtHeader>(Base64DecodeToString(split[0]));

                string jsonData = Base64DecodeToString(split[1]);

                //byte[] verficationBytes = EncodingHelper.GetBytes(Base64DecodeToString(split[2]));
                string verification = split[2];

                return (jsonHeaderData, jsonData, verification);
            }
            else
            {
                throw new InvalidTokenPartsException("token");
            }
        }

        /// <summary>
        /// Decodes the payload into the provided type.
        /// </summary>
        /// <returns>The payload.</returns>
        /// <param name="token">A properly formatted .</param>
        /// <typeparam name="T">The type you wish to decode into.</typeparam>
        public static T DecodePayload<T>(string token)
        {
            var payloadDecoded = JsonConvert.DeserializeObject<T>(DecodeToken(token).Payload);
            return payloadDecoded;
        }

        private static string Base64DecodeToString(string ToDecode)
        {
            string decodePrepped = ToDecode.Replace("-", "+").Replace("_", "/");

            switch (decodePrepped.Length % 4)
            {
                case 0:
                    break;
                case 2:
                    decodePrepped += "==";
                    break;
                case 3:
                    decodePrepped += "=";
                    break;
                default:
                    throw new Exception("Not a legal base64 string!");
            }

            byte[] data = Convert.FromBase64String(decodePrepped);
            return System.Text.Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Is the token expired?
        /// </summary>
        /// <returns><c>true</c>, if expired, <c>false</c> otherwise.</returns>
        /// <param name="token">Token.</param>
        //public static bool IsExpired(string token)
        //{
        //    var tokenDecoded = DecodeToken(token);
        //    var expiration = JsonConvert.DeserializeObject<JwtExpiration>(tokenDecoded.Payload).Expiration;

        //    bool isExpired = expiration != null;

        //    if (expiration != null)
        //    {
        //        isExpired = DateTimeHelpers.FromUnixTime((long)expiration) > DateTime.Now;
        //    }

        //    return isExpired;
        //}
    }
    public class InvalidTokenPartsException : ArgumentOutOfRangeException
    {
        /// <summary>
        /// Creates an instance of <see cref="InvalidTokenPartsException" />
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception</param>
        public InvalidTokenPartsException(string paramName)
            : base(paramName, "Token must consist of 3 delimited by dot parts.")
        {
        }
    }
}

