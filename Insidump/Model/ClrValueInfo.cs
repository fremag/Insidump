﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Insidump.Model;

[PrimaryKey(nameof(Address))]
public class ClrValueInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public ulong Address { get; init; }

    public int ClrTypeId { get; init; }
}