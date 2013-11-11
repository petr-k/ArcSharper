using System.Collections.Generic;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Psi;
using IObjectCreationExpression = JetBrains.ReSharper.Psi.CSharp.Tree.IObjectCreationExpression;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using System.Linq;
using JetBrains.ReSharper.Psi.Util;

namespace ArcSharper
{
    [ElementProblemAnalyzer(new[] { typeof(IObjectCreationExpression) }, HighlightingTypes = new[] { typeof(DangerousAoSingletonInstantiationHighlighting) })]
    public class ArcObjectsAnalyzer : ElementProblemAnalyzer<IObjectCreationExpression>
    {
        private static readonly IDictionary<string, string> SingletonCoClassNames = new Dictionary<string, string>
        {
            { "ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactoryClass", "ESRI.ArcGIS.Geodatabase.IWorkspaceFactory" },
            { "ESRI.ArcGIS.DataSourcesGDB.InMemoryWorkspaceFactoryClass", "ESRI.ArcGIS.Geodatabase.IWorkspaceFactory" },
            { "ESRI.ArcGIS.Geoprocessing.GPUtilitiesClass", "ESRI.ArcGIS.Geoprocessing.IGPUtilities" },
        };

        protected override void Run(IObjectCreationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            var expressionType = element.GetExpressionType();

            if (!expressionType.IsResolved)
            {
                return;
            }

            var type = expressionType.ToIType() as IDeclaredType;
            if (type == null)
            {
                return;
            }

            var typeElement = type.GetTypeElement();
            var attributesOwner = typeElement as IAttributesOwner;

            if (typeElement == null)
            {
                return;
            }

            var comImportAttribute = TypeFactory.CreateTypeByCLRName("System.Runtime.InteropServices.ComImportAttribute", element.GetPsiModule());

            if (!attributesOwner.HasAttributeInstance(comImportAttribute.GetClrName(), false))
            {
                return;
            }

            var clrName = type.GetClrName();
            string interfaceTypeName;
            if (SingletonCoClassNames.TryGetValue(clrName.FullName, out interfaceTypeName))
            {
                consumer.AddHighlighting(new DangerousAoSingletonInstantiationHighlighting(element, typeElement, interfaceTypeName));
                return;
            }

            var coClassAttribute = TypeFactory.CreateTypeByCLRName("System.Runtime.InteropServices.CoClassAttribute", element.GetPsiModule());
            var attributes = attributesOwner.GetAttributeInstances(false);
            var coClassAttributeInstance = attributes.FirstOrDefault(a => a.AttributeType.Equals(coClassAttribute));

            if (coClassAttributeInstance == null || coClassAttributeInstance.PositionParameterCount == 0)
            {
                return;
            }

            var coClassAttributeValue = coClassAttributeInstance.PositionParameter(0);
            typeElement = coClassAttributeValue.TypeValue.GetTypeElement<ITypeElement>();

            if (typeElement == null)
            {
                return;
            }

            clrName = typeElement.GetClrName();
            if (SingletonCoClassNames.TryGetValue(clrName.FullName, out interfaceTypeName))
            {
                consumer.AddHighlighting(new DangerousAoSingletonInstantiationHighlighting(element, typeElement, interfaceTypeName));
            }
        }
    }
}
