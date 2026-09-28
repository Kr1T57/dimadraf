using System;
using System.Collections.Generic;

namespace GroupWorkAPI;

public partial class VariantImage
{
    public int Id { get; set; }

    public int ProductVariantId { get; set; }

    public string ImagePath { get; set; } = null!;

    public virtual ProductVariant ProductVariant { get; set; } = null!;

    public VariantImage ToDto()
    {
        return new()
        {
            Id = Id,
            ProductVariantId = ProductVariant.Id,
            ImagePath = ImagePath
        };
    }

}
