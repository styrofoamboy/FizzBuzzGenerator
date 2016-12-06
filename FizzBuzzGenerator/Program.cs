using System;
using System.Collections.Generic;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;

namespace FizzBuzzGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            CompilerInfo[] langInfo = CodeDomProvider.GetAllCompilerInfo();
            Console.WriteLine("Please choose a language:");
            Console.WriteLine("".PadLeft(30, '='));
            for (int i = 0; i < langInfo.Length; i++)
                if (langInfo[i].IsCodeDomProviderTypeValid)
                    Console.WriteLine("{0} - {1}", i + 1, langInfo[i].CodeDomProviderType.Name);
            Console.WriteLine();
            ConsoleKeyInfo nullKey = new ConsoleKeyInfo('.', ConsoleKey.Decimal, false, false, false);
            ConsoleKeyInfo langSelKey = nullKey;
            while (langSelKey == nullKey)
            {
                Console.Write("\nPlease choose: ");
                ConsoleKeyInfo tempKeyInfo = Console.ReadKey();
                int iKeyCharVal;
                if (int.TryParse(tempKeyInfo.KeyChar.ToString(), out iKeyCharVal) && iKeyCharVal > 0 && iKeyCharVal <= langInfo.Length)
                    langSelKey = tempKeyInfo;
                else
                    Console.WriteLine("?");
            }
            int iKeyCharValAgain = -1;
            if (langSelKey.KeyChar != null)
                int.TryParse(langSelKey.KeyChar.ToString(), out iKeyCharValAgain);

            bool compile = false;
            Console.Write('\n');
            Console.Write('\n');
            ConsoleKeyInfo nullKey2 = new ConsoleKeyInfo('0', ConsoleKey.NumPad0, false, false, false);
            ConsoleKeyInfo chooseCompileKey = nullKey2;
            while (chooseCompileKey == nullKey2)
            {
                Console.Write('\n');
                Console.Write("Compile output? ");
                ConsoleKeyInfo tempKeyInfo2 = Console.ReadKey();
                if (tempKeyInfo2.Key == ConsoleKey.Y || tempKeyInfo2.Key == ConsoleKey.N)
                    chooseCompileKey = tempKeyInfo2;
                else
                    Console.WriteLine("?");
            }
            if (chooseCompileKey.Key == ConsoleKey.Y)
                compile = true;
            else
                compile = false;

            bool memberLineBreaks = false;
            Console.Write("\n\n");
            ConsoleKeyInfo chooseLineBreaks = nullKey2;
            while (chooseLineBreaks == nullKey2)
            {
                Console.Write("\n");
                Console.Write("Line breaks in output? ");
                ConsoleKeyInfo tempKeyInfo3 = Console.ReadKey();
                if (tempKeyInfo3.Key == ConsoleKey.Y || tempKeyInfo3.Key == ConsoleKey.N)
                    chooseLineBreaks = tempKeyInfo3;
                else
                    Console.WriteLine("?");
            }
            if (chooseLineBreaks.Key == ConsoleKey.Y)
                memberLineBreaks = true;

            int countMax = 0;
            Console.Write('\n');
            bool hasMaxValue = false;
            while (hasMaxValue == false)
            {
                Console.Write('\n');
                Console.Write("\nCount how high? ");
                string strMaxCnt = Console.ReadLine();
                if (!string.IsNullOrEmpty(strMaxCnt) && int.TryParse(strMaxCnt, out countMax))
                    hasMaxValue = true;
                else
                    Console.WriteLine("?");
            }

            // Now the fun part starts.
            CodeNamespace ns = new CodeNamespace("com.wtf.fizzbuzz");
            ns.Imports.Add(new CodeNamespaceImport("System"));

            // public class FizzBuzzOutputter {
            CodeTypeDeclaration fzbzCls = new CodeTypeDeclaration("FizzBuzzOutputter");
            fzbzCls.TypeAttributes = System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Class;
            fzbzCls.IsClass = true;

            // public int _MaxCount;
            CodeMemberField maxField = new CodeMemberField(
                                            new CodeTypeReference("System.Int32"), "_MaxCount");
            maxField.Attributes = MemberAttributes.Public;
            fzbzCls.Members.Add(maxField);

            // public FizzBuzzOutputter(int max) {
            CodeConstructor cstr = new CodeConstructor();
            cstr.Attributes = MemberAttributes.Public;
            cstr.Parameters.Add(
                    new CodeParameterDeclarationExpression(
                        new CodeTypeReference("System.Int32"), "max"));
            // this._MaxCount = max;
            cstr.Statements.Add(new CodeAssignStatement(
                new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(),
                    "_MaxCount"
                ), new CodeArgumentReferenceExpression("max")));
            fzbzCls.Members.Add(cstr);

            //public virtual string doMeth(int countMax) {
            CodeMemberMethod doMeth = new CodeMemberMethod();
            doMeth.Name = "doMeth";
            doMeth.Attributes = MemberAttributes.Public;
            doMeth.ReturnType = new CodeTypeReference("System.String");
            doMeth.Parameters.Add(
                new CodeParameterDeclarationExpression(
                    new CodeTypeReference("System.Int32"), "countMax")
                );

            // for (int i = 0; i < countMax; i++) {
            CodeIterationStatement forLooop = new CodeIterationStatement(
                new CodeVariableDeclarationStatement(
                    new CodeTypeReference(
                        new CodeTypeParameter("System.Int32")
                    ), "i", new CodePrimitiveExpression(0)
                ), new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression("i"),
                    CodeBinaryOperatorType.LessThan,
                    new CodeArgumentReferenceExpression("countMax")
                ), new CodeAssignStatement(
                    new CodeVariableReferenceExpression("i"),
                    new CodeBinaryOperatorExpression(
                        new CodeVariableReferenceExpression("i"),
                        CodeBinaryOperatorType.Add,
                        new CodePrimitiveExpression(1)
                    )
                ));

            // int x = (i + 1);
            CodeVariableDeclarationStatement varX = new CodeVariableDeclarationStatement(
                "System.Int32", "x", new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression("i"),
                    CodeBinaryOperatorType.Add,
                    new CodePrimitiveExpression(1)
                ));
            forLooop.Statements.Add(varX);

            // if (((x % 5) == 0) && ((x % 3) == 0)) {
            CodeConditionStatement ifFizzBuzz = new CodeConditionStatement();
            ifFizzBuzz.Condition = new CodeBinaryOperatorExpression(
                new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(
                        new CodeVariableReferenceExpression("x"),
                        CodeBinaryOperatorType.Modulus,
                        new CodePrimitiveExpression(5)
                    ), CodeBinaryOperatorType.ValueEquality,
                    new CodePrimitiveExpression(0)
                ), CodeBinaryOperatorType.BooleanAnd
                , new CodeBinaryOperatorExpression(
                    new CodeBinaryOperatorExpression(
                        new CodeVariableReferenceExpression("x"),
                        CodeBinaryOperatorType.Modulus,
                        new CodePrimitiveExpression(3)
                    ), CodeBinaryOperatorType.ValueEquality,
                    new CodePrimitiveExpression(0)
                ));
            
            // System.Console.WriteLine("FizzBuzz");
            ifFizzBuzz.TrueStatements.Add(
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(
                            new CodeTypeReference(typeof(Console))
                        ), "WriteLine", new CodePrimitiveExpression("FizzBuzz")
                    ));
            
            // else if ((x % 3) == 0) {
            CodeConditionStatement elseIfFizz = new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            new CodeBinaryOperatorExpression(
                                new CodeVariableReferenceExpression("x"),
                                CodeBinaryOperatorType.Modulus,
                                new CodePrimitiveExpression(3)
                            ), CodeBinaryOperatorType.ValueEquality,
                            new CodePrimitiveExpression(0)
                        ));
            ifFizzBuzz.FalseStatements.Add(elseIfFizz);
            
            // System.Console.Writeline("Fizz");
            elseIfFizz.TrueStatements.Add(
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(
                            new CodeTypeReference(typeof(Console))
                        ), "WriteLine", new CodePrimitiveExpression("Fizz")
                    ));

            // else if ((x % 5) == 0) {
            CodeConditionStatement elseIfBuzz = new CodeConditionStatement(
                        new CodeBinaryOperatorExpression(
                            new CodeBinaryOperatorExpression(
                                new CodeVariableReferenceExpression("x"),
                                CodeBinaryOperatorType.Modulus,
                                new CodePrimitiveExpression(5)
                            ), CodeBinaryOperatorType.ValueEquality,
                            new CodePrimitiveExpression(0)
                        ));
            elseIfFizz.FalseStatements.Add(elseIfBuzz);

            // System.Console.WriteLine("Buzz");
            elseIfBuzz.TrueStatements.Add(
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(
                            new CodeTypeReference(typeof(Console))
                        ), "WriteLine", new CodePrimitiveExpression("Buzz")
                    ));

            // else { System.Console.WriteLine(x); }
            elseIfBuzz.FalseStatements.Add(
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(
                            new CodeTypeReference(typeof(Console))
                        ), "WriteLine", new CodeVariableReferenceExpression("x")
                    ));

            forLooop.Statements.Add(ifFizzBuzz);
            doMeth.Statements.Add(forLooop);

            // return countMax.ToString();
            doMeth.Statements.Add(
                new CodeMethodReturnStatement(
                    new CodeMethodInvokeExpression(
                        new CodeArgumentReferenceExpression("countMax"),
                        "ToString"
                    )
                ));

            fzbzCls.Members.Add(doMeth);

            // public class Program {
            CodeTypeDeclaration progMainCls = new CodeTypeDeclaration("Program");

            // System.Console.WriteLine("Press any key fizz/buzz...");
            CodeMethodInvokeExpression consWrite = new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(
                    new CodeTypeReferenceExpression(
                        new CodeTypeReference(typeof(Console))
                    ), "WriteLine"
                ), new CodePrimitiveExpression("Press any key to fizz/buzz..."));

            // System.Console.ReadLine();
            CodeMethodInvokeExpression consRead = new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(
                    new CodeTypeReferenceExpression(
                        new CodeTypeReference(typeof(Console))
                    ), "ReadLine"
                ));

            // public static void Main() {
            CodeEntryPointMethod voidMan = new CodeEntryPointMethod();
            voidMan.Parameters.Add(new CodeParameterDeclarationExpression(
                new CodeTypeReference(typeof(object[])), "args"));

            // Add "press any key" statement from above.
            voidMan.Statements.Add(consWrite);

            // Add "ReadLine()" state from above.
            voidMan.Statements.Add(consRead);

            // FuzzBuzzOutputter inst = new FizzBuzzOutputter(<count>);
            voidMan.Statements.Add(
                new CodeVariableDeclarationStatement(
                    new CodeTypeReference("FizzBuzzOutputter"),
                    "inst",
                    new CodeObjectCreateExpression(
                        new CodeTypeReference("FizzBuzzOutputter"),
                        new CodePrimitiveExpression(countMax)
                    )
                ));

            // int.doMeth(<count>);
            voidMan.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeVariableReferenceExpression("inst"),
                    "doMeth",
                    new CodePrimitiveExpression(countMax)
                ));

            // System.Console.WriteLine("Press any key to exit...");
            voidMan.Statements.Add(
                new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(
                    new CodeTypeReferenceExpression(
                        new CodeTypeReference(typeof(Console))
                    ), "WriteLine"
                ), new CodePrimitiveExpression("Press any key to exit...")));   // And by "any key", I mean <Enter> :)

            // Add "ReadLine()" state from above.
            voidMan.Statements.Add(consRead);

            // Add "Main()" to class.
            progMainCls.Members.Add(voidMan);

            // Add classes to namespace.
            ns.Types.Add(progMainCls);
            ns.Types.Add(fzbzCls);

            // Create a CompileUnit.
            CodeCompileUnit cdCmpUnt = new CodeCompileUnit();
            cdCmpUnt.Namespaces.Add(ns);

            // Finally ready to output the result.
            CodeDomProvider provider = langInfo[iKeyCharValAgain - 1].CreateProvider();
            CodeGeneratorOptions opts = new CodeGeneratorOptions();
            opts.BlankLinesBetweenMembers = memberLineBreaks;
            opts.ElseOnClosing = true;

            StringBuilder sbOutput = new StringBuilder();
            using (System.IO.StringWriter sr = new System.IO.StringWriter(sbOutput))
                provider.GenerateCodeFromCompileUnit(cdCmpUnt, sr, opts);

            Console.WriteLine(sbOutput);

            Console.Write('\n');
            Console.Write('\n');
            Console.Write("\nSpecify file name: ");
            string outFileNm = Console.ReadLine();
            string outPath = System.IO.Path.Combine(Environment.CurrentDirectory, outFileNm);
            if (compile)
            {
                try
                {
                    CompilerParameters compParams = new CompilerParameters();
                    compParams.ReferencedAssemblies.Add("System.dll");
                    compParams.WarningLevel = 3;
                    compParams.CompilerOptions = "/optimize";
                    compParams.GenerateExecutable = true;
                    compParams.IncludeDebugInformation = false;
                    compParams.GenerateInMemory = false;
                    if (provider.Supports(GeneratorSupport.EntryPointMethod))
                        compParams.MainClass = "com.wtf.fizzbuzz.Program";
                    compParams.OutputAssembly = outPath + ".exe";
                    CompilerResults compResult = provider.CompileAssemblyFromDom(compParams, cdCmpUnt);
                    if (compResult.Errors.Count == 0)
                        Console.WriteLine("File created at: " + compResult.PathToAssembly);
                    else
                    {
                        Console.WriteLine("This following errors occured durring compilation:");
                        for (int i = 0; i < compResult.Errors.Count; i++)
                            Console.WriteLine("\t- {0}", compResult.Errors[i].ErrorText);
                    }
                }
                catch (Exception ex)
                { Console.WriteLine("An error occured while trying to compile FizzBuzz: " + ex.Message); }
            }
            else if (!string.IsNullOrWhiteSpace(outFileNm))
            {
                try
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(outPath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    using (System.IO.StreamWriter sr = new System.IO.StreamWriter(fs))
                        sr.Write(sbOutput.ToString());
                }
                catch (Exception ex)
                { Console.Write("An error occured while trying to write source code file: " + ex.Message); }
            }

            Console.WriteLine();
            Console.WriteLine("".PadLeft(30, '='));
            Console.WriteLine("ALL DONE!  Press <Enter> to exit.");
            Console.ReadLine();
        }
    }
}
