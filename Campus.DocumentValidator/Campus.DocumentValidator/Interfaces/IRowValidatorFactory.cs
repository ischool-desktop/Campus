using System.Xml;
namespace Campus.DocumentValidator
{
    public interface IRowValidatorFactory
    {
        IRowVaildator CreateRowValidator(string typeName, XmlElement validatorDescription);
    }
}