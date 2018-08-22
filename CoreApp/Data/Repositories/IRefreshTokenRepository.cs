﻿using System;
using System.Threading.Tasks;
using Common.Models;

namespace Data.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<Token> Upsert(Token token);
        Task Delete(object id);
        Task Delete(Func<Token, bool> condition);
    }
}