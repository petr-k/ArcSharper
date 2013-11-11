using System;
using JetBrains.ActionManagement;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.LinqTools;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using JetBrains.UI.PopupWindowManager;
using JetBrains.UI.Tooltips;
using JetBrains.Util;

namespace ArcSharper
{
    [QuickFix]
    public class DangerousAoSingletonInstantiationQuickFix : QuickFixBase
    {
        private readonly DangerousAoSingletonInstantiationHighlighting _highlighting;

        public DangerousAoSingletonInstantiationQuickFix([NotNull] DangerousAoSingletonInstantiationHighlighting highlighting)
        {
            if (highlighting == null) throw new ArgumentNullException("highlighting");
            _highlighting = highlighting;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            if (_highlighting.ObjectCreationExpression.Initializer != null)
            {
                return textControl =>
                       JetBrains.UI.Application.ShellComponentsEx.Tooltips(Shell.Instance.Components)
                                .Show("Not supported for object initializers", l => (IPopupWindowContext)new TextControlPopupWindowContext(l, textControl, Shell.Instance.GetComponent<IShellLocks>(), Shell.Instance.GetComponent<IActionManager>()));
            }

            var psiModule = _highlighting.ObjectCreationExpression.GetPsiModule();
            var factory = CSharpElementFactory.GetInstance(psiModule);

            var activatorType = TypeFactory.CreateTypeByCLRName("System.Activator", psiModule);
            var typeType = TypeFactory.CreateTypeByCLRName("System.Type", psiModule);
            var intefaceType = TypeFactory.CreateTypeByCLRName(_highlighting.InterfaceTypeName, psiModule);

            var statement = factory.CreateExpression("($3)$0.CreateInstance($1.GetTypeFromCLSID(typeof($2).GUID))",
                                                     activatorType.GetTypeElement(),
                                                     typeType.GetTypeElement(),
                                                     _highlighting.CoClassTypeElement,
                                                     intefaceType
                );

            var containingDeclarationStatement = _highlighting.ObjectCreationExpression.GetContainingStatement() as IDeclarationStatement;
            if (containingDeclarationStatement != null && containingDeclarationStatement.Declaration.Declarators.Count == 1)
            {
                containingDeclarationStatement.Declaration.Declarators[0].SetTypeOrVar(intefaceType);
            }

            _highlighting.ObjectCreationExpression.ReplaceBy(statement);

            return null;
        }

        public override string Text
        {
            get { return "Instantiate via Activator.CreateInstance"; }
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            return true;
        }
    }
}