﻿using System.Buffers.Text;

namespace NetCord;

public class Token
{
    public Token(TokenType type, string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException($"'{nameof(token)}' cannot be null or empty.", nameof(token));

        Type = type;
        RawToken = token;
    }

    public TokenType Type { get; }

    public string RawToken { get; }

    public ulong Id
    {
        get
        {
            int index = RawToken.IndexOf('.');
            if (index != -1)
            {
                var idBase64 = RawToken[..index];
                var idConverted = Convert.FromBase64String(idBase64.PadRight((idBase64.Length + 3) / 4 * 4, '='));
                if (Utf8Parser.TryParse(idConverted, out ulong id, out int bytesConsumed) && idConverted.Length == bytesConsumed)
                    return id;
            }

            throw new InvalidOperationException("Invalid token provided.");
        }
    }

    public string ToHttpHeader() => Type switch
    {
        TokenType.Bot => $"Bot {RawToken}",
        TokenType.Bearer => $"Bearer {RawToken}",
        _ => RawToken,
    };
}
