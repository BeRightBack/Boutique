using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Boutique.Middleware;
/// <summary>
/// Decimal precision validator data annotation.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class DecimalPrecisionAttribute : ValidationAttribute
{
  private readonly uint _decimalPrecision;

  public DecimalPrecisionAttribute(uint decimalPrecision)
  {
    _decimalPrecision = decimalPrecision;
  }

  public override bool IsValid(object value)
  {
    return value is null || (value is decimal d && HasPrecision(d, _decimalPrecision));
  }

  private static bool HasPrecision(decimal value, uint precision)
  {
    string valueStr = value.ToString(CultureInfo.InvariantCulture);
    int indexOfDot = valueStr.IndexOf('.');
    if (indexOfDot == -1)
    {
      return true;
    }

    return valueStr.Length - indexOfDot - 1 <= precision;
  }
}