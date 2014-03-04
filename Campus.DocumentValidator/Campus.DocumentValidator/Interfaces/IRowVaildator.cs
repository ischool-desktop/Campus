using System.Xml;

namespace Campus.DocumentValidator
{
    public interface IRowVaildator
    {
        bool Validate(IRowStream Value);

        string Correct(IRowStream Value);

        string ToString(string template);
    }
}