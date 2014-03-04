using System.Xml;
namespace Campus.DocumentValidator
{
    public interface IFieldValidatorFactory
    {
        IFieldValidator CreateFieldValidator(string typeName,XmlElement validatorDescription);
    }
}