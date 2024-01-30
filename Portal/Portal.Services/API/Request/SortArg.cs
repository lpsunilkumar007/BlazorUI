﻿namespace Portal.Services.API.Request
{
    public record SortArg(string Member, bool IsAscending)
    {
        public override string ToString()
        {
            var direction = IsAscending ? "asc" : "desc";
            return $"{Member}:{direction}";
        }
    }
}
