using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.ClientManifestClassGenerator
{
    public class ClassGeneratorService
    {
        public async Task<string> GenerateClassFromJsonSchemaAsync(string jsonSchema, string namespaceName, string rootClassName)
        {
            // Load the JSON schema
            var schema = await JsonSchema.FromJsonAsync(jsonSchema);

            // Configure the CSharpGeneratorSettings
            var settings = new CSharpGeneratorSettings
            {
                Namespace = namespaceName,
                ClassStyle = CSharpClassStyle.Poco,
                GenerateDataAnnotations = false,
                //TemplateFactory = new CustomTemplateFactory()
            };

            // Generate C# class from the schema with the specified namespace and root class name
            var generator = new CSharpGenerator(schema, settings);
            var file = generator.GenerateFile(rootClassName);

            return file;
        }
    }

    //public class CustomTemplateFactory : ITemplateFactory
    //{
    //    public ITemplate CreateTemplate(string language, string template, object model)
    //    {
    //        if (template == "Class")
    //        {
    //            return new CustomClassTemplate((CSharpClassTemplateModel)model);
    //        }

    //        return new DefaultTemplateFactory().CreateTemplate(template, model);
    //    }
    //}

    //public class CustomClassTemplate : ITemplate
    //{
    //    private readonly CSharpClassTemplateModel _model;

    //    public CustomClassTemplate(CSharpClassTemplateModel model)
    //    {
    //        _model = model;
    //    }

    //    public string Render()
    //    {
    //        var className = _model.Class;
    //        var interfaceName = $"I{className}";

    //        var sb = new StringBuilder();
    //        sb.AppendLine($"namespace {_model.Namespace}");
    //        sb.AppendLine("{");
    //        sb.AppendLine($"    public interface {interfaceName} {{ }}");
    //        sb.AppendLine();
    //        sb.AppendLine($"    public partial class {className} : {interfaceName}");
    //        sb.AppendLine("    {");
    //        sb.AppendLine("        // Class properties and methods");
    //        sb.AppendLine("    }");
    //        sb.AppendLine("}");

    //        return sb.ToString();
    //    }
    //}
}
