﻿
namespace Portal.Shared.Interfaces;

public interface IId
{
    int Id { get; set; }
}

public interface IName
{
    string Name { get; set; }
}

public interface IIdName<out TId>
{
    TId Id { get; }
    string Name { get; }
}
public interface IIdName : IId, IName { }
