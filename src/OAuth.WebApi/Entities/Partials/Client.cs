using System;

// ReSharper disable once CheckNamespace
namespace OAuth.WebApi.Entities
{
    public partial class Client
    {
        public TimeSpan TokenTimeSpan => TimeSpan.FromMinutes(TokenLifeTime);
    }
}