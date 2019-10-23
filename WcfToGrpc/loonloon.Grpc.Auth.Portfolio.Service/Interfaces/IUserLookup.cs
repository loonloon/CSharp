using System;

namespace loonloon.Grpc.Auth.Portfolio.Service.Interfaces
{
    public interface IUserLookup
    {
        bool TryGetId(string name, out Guid guid);
    }
}
