//
// DelegateSubtractionIssue.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System.Collections.Generic;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Delegate subtractions",
		Description = "Delegate subtraction has unpredictable result",
		Category = IssueCategories.CodeQualityIssues,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "DelegateSubtraction")]
	public class DelegateSubtractionIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseSemanticModel context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<DelegateSubtractionIssue>
		{
			public GatherVisitor(BaseSemanticModel ctx) : base (ctx)
			{
			}

			public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
			{
				base.VisitBinaryOperatorExpression(binaryOperatorExpression);
				if (binaryOperatorExpression.Operator != BinaryOperatorType.Subtract)
					return;
				var rr = ctx.Resolve(binaryOperatorExpression.Right);
				if (rr.Type.Kind == ICSharpCode.NRefactory.TypeSystem.TypeKind.Delegate && !IsEvent (binaryOperatorExpression.Left)) {
					AddIssue(new CodeIssue(binaryOperatorExpression, ctx.TranslateString("Delegate subtraction has unpredictable result")));
				}
			}

			bool IsEvent(AstNode node)
			{
				var rr = ctx.Resolve(node) as MemberResolveResult;
				return rr != null && rr.Member.SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Event;
			}

			public override void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
			{
				base.VisitAssignmentExpression(assignmentExpression);
				if (assignmentExpression.Operator != AssignmentOperatorType.Subtract)
					return;
				var rr = ctx.Resolve(assignmentExpression.Right);
				if (rr.Type.Kind == ICSharpCode.NRefactory.TypeSystem.TypeKind.Delegate && !IsEvent (assignmentExpression.Left))
					AddIssue(new CodeIssue(assignmentExpression, ctx.TranslateString("Delegate subtraction has unpredictable result")));
			}
		}
	}
}

