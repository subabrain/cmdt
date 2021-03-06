//==========================================================================
//
//  File:        BSMB.m
//  Location:    FileSystem <Oslo MGrammar>
//  Description: BSMB语法
//  Version:     2009.08.22.
//  Copyright(C) F.R.C.
//
//==========================================================================

module Commandos{
    @{CaseInsensitive}
    language BSMB{
        token NewLine = '\n' | '\r' '\n';
        @{Classification["Comment"]}
        token LineComment = "#" ^('\r' | '\n')* NewLine;
        token BlockComment = "/*" ^('/' | '*')* "*/";
        interleave Whitespace = NewLine | " " | LineComment | BlockComment;

        token StringIdentifier = ('A'..'Z' | 'a'..'z' | '0'..'9' | '_' | '<' | '>' | '@')+;
        token StringNormalHeadChar = 'A'..'Z' | 'a'..'z' | '_' | '!' | '$' | ':' | '%' | '=' | '<' | '>' | '@' | '/' | '\\';
        token StringNormalChar = StringNormalHeadChar | '.' | '-' ;
        token StringNumericChar = '0'..'9';
        token StringLiteral = StringNormalHeadChar (StringNormalChar | StringNumericChar)*
                            | (StringNormalHeadChar | StringNumericChar) (StringNormalChar | StringNumericChar)* StringNormalChar+ (StringNormalChar | StringNumericChar)*;
        token NumericLiteral = ('+' | '-')? (('0'..'9')+ | ('0'..'9')* '.' ('0'..'9')*);
        token FieldToken = '.' s:StringIdentifier => s;

        syntax Value = v:NumericLiteral => v
                     | v:StringLiteral => v
                     | v:Array => v
                     | v:Structure => v;
        syntax Array = '(' e:Value* ')' => Array[valuesof(e)];
        syntax Structure = '[' f:Field* ']' => Structure{Fields[valuesof(f)]};
        syntax Field = n:FieldToken v:Value => Field{Name => n, Value => v}
                     | n:FieldToken v:Value+ => Field{Name => n, Value => Array[valuesof(v)]};

        syntax Main = Structure;
    }
}
