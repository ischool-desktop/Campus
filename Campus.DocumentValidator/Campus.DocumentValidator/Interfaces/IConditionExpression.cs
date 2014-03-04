namespace Campus.DocumentValidator
{
    public interface IConditionExpression
    {
        bool Evaluate(IRowStream RowSource);
    }
}