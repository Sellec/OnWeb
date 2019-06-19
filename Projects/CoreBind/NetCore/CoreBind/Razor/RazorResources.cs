using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace System.Web.Razor.Resources
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	internal class RazorResources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(RazorResources.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("System.Web.Razor.Resources.RazorResources", typeof(RazorResources).Assembly);
					RazorResources.resourceMan = resourceManager;
				}
				return RazorResources.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return RazorResources.resourceCulture;
			}
			set
			{
				RazorResources.resourceCulture = value;
			}
		}

		internal static string ActiveParser_Must_Be_Code_Or_Markup_Parser
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ActiveParser_Must_Be_Code_Or_Markup_Parser", RazorResources.resourceCulture);
			}
		}

		internal static string Block_Type_Not_Specified
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Block_Type_Not_Specified", RazorResources.resourceCulture);
			}
		}

		internal static string BlockName_Code
		{
			get
			{
				return RazorResources.ResourceManager.GetString("BlockName_Code", RazorResources.resourceCulture);
			}
		}

		internal static string BlockName_ExplicitExpression
		{
			get
			{
				return RazorResources.ResourceManager.GetString("BlockName_ExplicitExpression", RazorResources.resourceCulture);
			}
		}

		internal static string CancelBacktrack_Must_Be_Called_Within_Lookahead
		{
			get
			{
				return RazorResources.ResourceManager.GetString("CancelBacktrack_Must_Be_Called_Within_Lookahead", RazorResources.resourceCulture);
			}
		}

		internal static string CreateCodeWriter_NoCodeWriter
		{
			get
			{
				return RazorResources.ResourceManager.GetString("CreateCodeWriter_NoCodeWriter", RazorResources.resourceCulture);
			}
		}

		internal static string CSharpSymbol_CharacterLiteral
		{
			get
			{
				return RazorResources.ResourceManager.GetString("CSharpSymbol_CharacterLiteral", RazorResources.resourceCulture);
			}
		}

		internal static string CSharpSymbol_Comment
		{
			get
			{
				return RazorResources.ResourceManager.GetString("CSharpSymbol_Comment", RazorResources.resourceCulture);
			}
		}

		internal static string CSharpSymbol_Identifier
		{
			get
			{
				return RazorResources.ResourceManager.GetString("CSharpSymbol_Identifier", RazorResources.resourceCulture);
			}
		}

		internal static string CSharpSymbol_IntegerLiteral
		{
			get
			{
				return RazorResources.ResourceManager.GetString("CSharpSymbol_IntegerLiteral", RazorResources.resourceCulture);
			}
		}

		internal static string CSharpSymbol_Keyword
		{
			get
			{
				return RazorResources.ResourceManager.GetString("CSharpSymbol_Keyword", RazorResources.resourceCulture);
			}
		}

		internal static string CSharpSymbol_Newline
		{
			get
			{
				return RazorResources.ResourceManager.GetString("CSharpSymbol_Newline", RazorResources.resourceCulture);
			}
		}

		internal static string CSharpSymbol_RealLiteral
		{
			get
			{
				return RazorResources.ResourceManager.GetString("CSharpSymbol_RealLiteral", RazorResources.resourceCulture);
			}
		}

		internal static string CSharpSymbol_StringLiteral
		{
			get
			{
				return RazorResources.ResourceManager.GetString("CSharpSymbol_StringLiteral", RazorResources.resourceCulture);
			}
		}

		internal static string CSharpSymbol_Whitespace
		{
			get
			{
				return RazorResources.ResourceManager.GetString("CSharpSymbol_Whitespace", RazorResources.resourceCulture);
			}
		}

		internal static string EndBlock_Called_Without_Matching_StartBlock
		{
			get
			{
				return RazorResources.ResourceManager.GetString("EndBlock_Called_Without_Matching_StartBlock", RazorResources.resourceCulture);
			}
		}

		internal static string ErrorComponent_Character
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ErrorComponent_Character", RazorResources.resourceCulture);
			}
		}

		internal static string ErrorComponent_EndOfFile
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ErrorComponent_EndOfFile", RazorResources.resourceCulture);
			}
		}

		internal static string ErrorComponent_Newline
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ErrorComponent_Newline", RazorResources.resourceCulture);
			}
		}

		internal static string ErrorComponent_Whitespace
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ErrorComponent_Whitespace", RazorResources.resourceCulture);
			}
		}

		internal static string HtmlSymbol_NewLine
		{
			get
			{
				return RazorResources.ResourceManager.GetString("HtmlSymbol_NewLine", RazorResources.resourceCulture);
			}
		}

		internal static string HtmlSymbol_RazorComment
		{
			get
			{
				return RazorResources.ResourceManager.GetString("HtmlSymbol_RazorComment", RazorResources.resourceCulture);
			}
		}

		internal static string HtmlSymbol_Text
		{
			get
			{
				return RazorResources.ResourceManager.GetString("HtmlSymbol_Text", RazorResources.resourceCulture);
			}
		}

		internal static string HtmlSymbol_WhiteSpace
		{
			get
			{
				return RazorResources.ResourceManager.GetString("HtmlSymbol_WhiteSpace", RazorResources.resourceCulture);
			}
		}

		internal static string Language_Does_Not_Support_RazorComment
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Language_Does_Not_Support_RazorComment", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_AtInCode_Must_Be_Followed_By_Colon_Paren_Or_Identifier_Start
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_AtInCode_Must_Be_Followed_By_Colon_Paren_Or_Identifier_Start", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_BlockComment_Not_Terminated
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_BlockComment_Not_Terminated", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_BlockNotTerminated
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_BlockNotTerminated", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Expected_CloseBracket_Before_EOF
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Expected_CloseBracket_Before_EOF", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Expected_EndOfBlock_Before_EOF
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Expected_EndOfBlock_Before_EOF", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Expected_X
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Expected_X", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Helpers_Cannot_Be_Nested
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Helpers_Cannot_Be_Nested", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_InheritsKeyword_Must_Be_Followed_By_TypeName
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_InheritsKeyword_Must_Be_Followed_By_TypeName", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_InlineMarkup_Blocks_Cannot_Be_Nested
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_InlineMarkup_Blocks_Cannot_Be_Nested", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_InvalidOptionValue
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_InvalidOptionValue", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_MarkupBlock_Must_Start_With_Tag
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_MarkupBlock_Must_Start_With_Tag", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_MissingCharAfterHelperName
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_MissingCharAfterHelperName", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_MissingCharAfterHelperParameters
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_MissingCharAfterHelperParameters", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_MissingEndTag
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_MissingEndTag", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_MissingOpenBraceAfterSection
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_MissingOpenBraceAfterSection", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_NamespaceImportAndTypeAlias_Cannot_Exist_Within_CodeBlock
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_NamespaceImportAndTypeAlias_Cannot_Exist_Within_CodeBlock", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_NamespaceOrTypeAliasExpected
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_NamespaceOrTypeAliasExpected", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_OuterTagMissingName
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_OuterTagMissingName", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_RazorComment_Not_Terminated
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_RazorComment_Not_Terminated", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_ReservedWord
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_ReservedWord", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Sections_Cannot_Be_Nested
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Sections_Cannot_Be_Nested", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_SingleLine_ControlFlowStatements_Not_Allowed
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_SingleLine_ControlFlowStatements_Not_Allowed", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_TextTagCannotContainAttributes
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_TextTagCannotContainAttributes", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Unexpected
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Unexpected", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Unexpected_Character_At_Helper_Name_Start
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Unexpected_Character_At_Helper_Name_Start", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Unexpected_Character_At_Section_Name_Start
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Unexpected_Character_At_Section_Name_Start", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Unexpected_Character_At_Start_Of_CodeBlock_CS
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Unexpected_Character_At_Start_Of_CodeBlock_CS", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Unexpected_Character_At_Start_Of_CodeBlock_VB
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Unexpected_Character_At_Start_Of_CodeBlock_VB", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Unexpected_EndOfFile_At_Start_Of_CodeBlock
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Unexpected_EndOfFile_At_Start_Of_CodeBlock", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Unexpected_Keyword_After_At
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Unexpected_Keyword_After_At", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Unexpected_Nested_CodeBlock
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Unexpected_Nested_CodeBlock", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Unexpected_WhiteSpace_At_Start_Of_CodeBlock_CS
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Unexpected_WhiteSpace_At_Start_Of_CodeBlock_CS", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Unexpected_WhiteSpace_At_Start_Of_CodeBlock_VB
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Unexpected_WhiteSpace_At_Start_Of_CodeBlock_VB", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_UnexpectedEndTag
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_UnexpectedEndTag", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_UnfinishedTag
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_UnfinishedTag", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_UnknownOption
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_UnknownOption", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_Unterminated_String_Literal
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_Unterminated_String_Literal", RazorResources.resourceCulture);
			}
		}

		internal static string ParseError_UnterminatedHelperParameterList
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParseError_UnterminatedHelperParameterList", RazorResources.resourceCulture);
			}
		}

		internal static string Parser_Context_Not_Set
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Parser_Context_Not_Set", RazorResources.resourceCulture);
			}
		}

		internal static string ParserContext_CannotCompleteTree_NoRootBlock
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParserContext_CannotCompleteTree_NoRootBlock", RazorResources.resourceCulture);
			}
		}

		internal static string ParserContext_CannotCompleteTree_OutstandingBlocks
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParserContext_CannotCompleteTree_OutstandingBlocks", RazorResources.resourceCulture);
			}
		}

		internal static string ParserContext_NoCurrentBlock
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParserContext_NoCurrentBlock", RazorResources.resourceCulture);
			}
		}

		internal static string ParserContext_ParseComplete
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParserContext_ParseComplete", RazorResources.resourceCulture);
			}
		}

		internal static string ParserEror_SessionDirectiveMissingValue
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParserEror_SessionDirectiveMissingValue", RazorResources.resourceCulture);
			}
		}

		internal static string ParserIsNotAMarkupParser
		{
			get
			{
				return RazorResources.ResourceManager.GetString("ParserIsNotAMarkupParser", RazorResources.resourceCulture);
			}
		}

		internal static string SectionExample_CS
		{
			get
			{
				return RazorResources.ResourceManager.GetString("SectionExample_CS", RazorResources.resourceCulture);
			}
		}

		internal static string SectionExample_VB
		{
			get
			{
				return RazorResources.ResourceManager.GetString("SectionExample_VB", RazorResources.resourceCulture);
			}
		}

		internal static string Structure_Member_CannotBeNull
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Structure_Member_CannotBeNull", RazorResources.resourceCulture);
			}
		}

		internal static string Symbol_Unknown
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Symbol_Unknown", RazorResources.resourceCulture);
			}
		}

		internal static string Tokenizer_CannotResumeSymbolUnlessIsPrevious
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Tokenizer_CannotResumeSymbolUnlessIsPrevious", RazorResources.resourceCulture);
			}
		}

		internal static string TokenizerView_CannotPutBack
		{
			get
			{
				return RazorResources.ResourceManager.GetString("TokenizerView_CannotPutBack", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_BackgroundThreadShutdown
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_BackgroundThreadShutdown", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_BackgroundThreadStart
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_BackgroundThreadStart", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_ChangesArrived
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_ChangesArrived", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_ChangesDiscarded
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_ChangesDiscarded", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_CollectedDiscardedChanges
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_CollectedDiscardedChanges", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_Disabled
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_Disabled", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_EditorProcessedChange
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_EditorProcessedChange", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_EditorReceivedChange
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_EditorReceivedChange", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_Enabled
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_Enabled", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_Format
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_Format", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_NoChangesArrived
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_NoChangesArrived", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_ParseComplete
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_ParseComplete", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_QueuingParse
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_QueuingParse", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_Startup
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_Startup", RazorResources.resourceCulture);
			}
		}

		internal static string Trace_TreesCompared
		{
			get
			{
				return RazorResources.ResourceManager.GetString("Trace_TreesCompared", RazorResources.resourceCulture);
			}
		}

		internal static string VBSymbol_CharacterLiteral
		{
			get
			{
				return RazorResources.ResourceManager.GetString("VBSymbol_CharacterLiteral", RazorResources.resourceCulture);
			}
		}

		internal static string VBSymbol_Comment
		{
			get
			{
				return RazorResources.ResourceManager.GetString("VBSymbol_Comment", RazorResources.resourceCulture);
			}
		}

		internal static string VBSymbol_DateLiteral
		{
			get
			{
				return RazorResources.ResourceManager.GetString("VBSymbol_DateLiteral", RazorResources.resourceCulture);
			}
		}

		internal static string VBSymbol_FloatingPointLiteral
		{
			get
			{
				return RazorResources.ResourceManager.GetString("VBSymbol_FloatingPointLiteral", RazorResources.resourceCulture);
			}
		}

		internal static string VBSymbol_Identifier
		{
			get
			{
				return RazorResources.ResourceManager.GetString("VBSymbol_Identifier", RazorResources.resourceCulture);
			}
		}

		internal static string VBSymbol_IntegerLiteral
		{
			get
			{
				return RazorResources.ResourceManager.GetString("VBSymbol_IntegerLiteral", RazorResources.resourceCulture);
			}
		}

		internal static string VBSymbol_Keyword
		{
			get
			{
				return RazorResources.ResourceManager.GetString("VBSymbol_Keyword", RazorResources.resourceCulture);
			}
		}

		internal static string VBSymbol_NewLine
		{
			get
			{
				return RazorResources.ResourceManager.GetString("VBSymbol_NewLine", RazorResources.resourceCulture);
			}
		}

		internal static string VBSymbol_RazorComment
		{
			get
			{
				return RazorResources.ResourceManager.GetString("VBSymbol_RazorComment", RazorResources.resourceCulture);
			}
		}

		internal static string VBSymbol_StringLiteral
		{
			get
			{
				return RazorResources.ResourceManager.GetString("VBSymbol_StringLiteral", RazorResources.resourceCulture);
			}
		}

		internal static string VBSymbol_WhiteSpace
		{
			get
			{
				return RazorResources.ResourceManager.GetString("VBSymbol_WhiteSpace", RazorResources.resourceCulture);
			}
		}

		internal RazorResources()
		{
		}
	}
}
