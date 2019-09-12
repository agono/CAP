﻿using System;
using System.Collections.Generic;

namespace DotNetCore.CAP.Messages
{
    public class Message
    {
        public Message(IDictionary<string, string> headers, object value)
        {
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IDictionary<string, string> Headers { get; }

        public object Value { get; }
    }

    public static class MessageExtensions
    {
        public static string GetId(this Message message)
        {
            message.Headers.TryGetValue(Headers.MessageId, out var value);
            return value;
        }

        public static string GetName(this Message message)
        {
            message.Headers.TryGetValue(Headers.MessageName, out var value);
            return value;
        }

        public static string GetCallbackName(this Message message)
        {
            message.Headers.TryGetValue(Headers.CallbackName, out var value);
            return value;
        }

        public static string GetGroup(this Message message)
        {
            message.Headers.TryGetValue(Headers.Group, out var value);
            return value;
        }

        public static int GetCorrelationSequence(this Message message)
        {
            if (message.Headers.TryGetValue(Headers.CorrelationSequence, out var value))
            {
                return int.Parse(value);
            }

            return 0;
        }
    }

}