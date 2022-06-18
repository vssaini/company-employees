﻿namespace Entities.Responses
{
    public class CompanyNotFoundResponse : ApiNotFoundResponse
    {
        public CompanyNotFoundResponse(Guid id) : base($"Company with id: {id} is not found in db.")
        {
        }
    }
}
