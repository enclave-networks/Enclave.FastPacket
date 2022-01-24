namespace Enclave.FastPacket.Generator.PositionProviders
{
    internal class AutomaticPositionProvider : IPositionProvider
    {
        public AutomaticPositionProvider(IPacketProperty? previousProperty)
        {
            PreviousProperty = previousProperty;
        }

        public IPacketProperty? PreviousProperty { get; }

        public virtual string GetPositionExpression(string spanName)
        {
            if (PreviousProperty is null)
            {
                // No previous property, we're starting from 0.
                return "0";
            }

            var previousPropPosition = PreviousProperty.PositionProvider.GetPositionExpression(spanName);

            // Automatic position calculation is just based on the position expression of the previous property,
            // plus the size (to take us to the start of this field).
            return $"{previousPropPosition} + {PreviousProperty.SizeProvider.GetSizeExpression(spanName, previousPropPosition)}";
        }
    }
}
