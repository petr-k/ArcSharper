using System;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ArcSharper
{
    [ConfigurableSeverityHighlighting("DangerousArcObjectSingletonInstantiation", "CSHARP", OverlapResolve = OverlapResolveKind.WARNING)]
    public class DangerousAoSingletonInstantiationHighlighting : IHighlightingWithRange
    {
        public DangerousAoSingletonInstantiationHighlighting([NotNull] IObjectCreationExpression objectCreationExpression,
                                                                    [NotNull] ITypeElement coClassTypeElement,
                                                                    [NotNull] string interfaceTypeName)
        {
            if (objectCreationExpression == null) throw new ArgumentNullException("objectCreationExpression");
            if (coClassTypeElement == null) throw new ArgumentNullException("coClassTypeElement");
            if (interfaceTypeName == null) throw new ArgumentNullException("interfaceTypeName");

            ObjectCreationExpression = objectCreationExpression;
            CoClassTypeElement = coClassTypeElement;
            InterfaceTypeName = interfaceTypeName;
        }

        public IObjectCreationExpression ObjectCreationExpression { get; private set; }

        public ITypeElement CoClassTypeElement { get; set; }

        public string InterfaceTypeName { get; set; }

        public bool IsValid()
        {
            return true;
        }

        public string ToolTip
        {
            get
            {
                return "Potentially dangerous ArcObject singleton instantation";
            }
        }

        public string ErrorStripeToolTip { get { return ToolTip; } }

        public int NavigationOffsetPatch { get { return 0; } }

        public DocumentRange CalculateRange()
        {
            return ObjectCreationExpression.GetDocumentRange();
        }
    }
}