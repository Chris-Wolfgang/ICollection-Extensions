namespace ShadowWorkloads;

/// <summary>One line of a shopping cart.</summary>
public sealed record OrderLine(int ProductId, int Quantity, bool Expired);
