/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

/// <summary>
///     Contains helper classes for p5 lambda conditional active events
/// </summary>
namespace p5.lambda.helpers
{
    /// <summary>
    ///     Class wrapping commonalities for conditional statements
    /// </summary>
    public class Conditions
    {
        // Used as buffer for all comparison operators and logical operators in system
        private static Node _operators;

        // Used to hold the number of operators used to evaluate the condition
        private int _noConditions = -1;

        /*
         * Recursively run through conditions
         */
        public bool Evaluate (ApplicationContext context, Node args)
        {
            // Looping through all conditional children nodes
            var conditions = GetConditionalEventNodes (context, args).ToList ();

            // Storing how many operators are part of our conditional statement, such that we can now the offset we should use later
            _noConditions = conditions.Count;

            // Looping through each condition
            foreach (var idx in conditions) {

                switch (idx.Name) {

                case "or":
                    TryEvaluateSimpleExist (context, args);
                    if (args.Get<bool> (context)) {

                        // Evaluated to true! Aborting the rest of conditional checks, since condition before [or] evaluated to true!
                        return true;
                    }

                    // Recursively loop through next condition, if previous condition did NOT evaluate to true!
                    args.Value = Evaluate (context, idx);
                    break;

                case "and":
                    TryEvaluateSimpleExist (context, args);

                    // Recursively loop through, but only if previous statements are true! If previous condition evaluated to false, we abort!
                    args.Value = args.Get<bool> (context) && Evaluate (context, idx);
                    break;

                case "xor":
                    TryEvaluateSimpleExist (context, args);

                    // Only evaluates to true if conditions are NOT EQUAL
                    args.Value = args.Get<bool> (context) != Evaluate (context, idx);
                    break;

                case "not":
                    TryEvaluateSimpleExist (context, args);

                    // Basic syntax checking
                    if (idx.Value != null || idx.Count != 0)
                        throw new LambdaException ("Operator [not] cannot have neither any value, nor any children", idx, context);

                    // Simply "negates" the previously evaluated condition
                    args.Value = !args.Get<bool> (context);
                    break;

                default:

                    // Raising comparison operator Active Event, 
                    // or any other Active Event currently part of conditional operators
                    context.RaiseLambda (idx.Name, idx);

                    // Moving results of Active Event invocation up from conditional Active Event invocation result node's value,
                    // to the result of conditional statement, to "bubble" results up to the top-most branching Active Event result
                    args.Value = idx.Value;
                    break;
                }
            }

            // If condition had no operator active event children, then we must evaluate a "simple exist" condition
            TryEvaluateSimpleExist (context, args);

            return args.Get<bool> (context);
        }

        /*
         * Executes current scope
         */
        public void ExecuteCurrentScope (ApplicationContext context, Node args)
        {
            // Making sure there actually is something to evaluate
            if (args.Count == 0)
                return;

            // Storing offset temporary in args, making sure we clean up afterwards
            args.Insert (0, new Node ("offset", _noConditions + 1 /* Remember [offset] node itself */));
            try
            {
                // Evaluating body of conditional statement, now with offset at first non-comparison operator event
                context.RaiseLambda ("eval-mutable", args);
            }
            finally
            {
                // Making sure we clean up, and remove our [offset], also in the case of exceptions being thrown
                args[0].UnTie ();
            }
        }

        /*
         * Will evaluate the given condition to true, if it is anything but a false boolean, null, 
         * or an expression returning anything but null or false
         */
        private void TryEvaluateSimpleExist (ApplicationContext context, Node args)
        {
            // If value is not boolean type, we evaluate value, and set its value to true, if evaluation did not
            // result in "null" or "false"
            if (args.Value == null) {

                // Null evaluates to false
                args.Value = false;
            } else {

                // Checking if value already is boolean, at which case we don't evaluate any further, since it is already evaluated
                if (!(args.Value is bool)) {

                    var obj = XUtil.Single<object> (context, args, false, null);
                    if (obj == null) {

                        // Result of evaluated expression yields null, hence evaluation result is false
                        args.Value = false;
                    } else if (obj is bool) {

                        // Result of evaluated expression yields boolean, using this boolean as result
                        args.Value = obj;
                    } else {

                        // Anything but null and boolean, existence is true, hence evaluation becomes true!
                        args.Value = true;
                    }
                }
            }
        }

        /*
         * Returns all nodes that either comparison operators or logical operators, and hence should be evaluated
         */
        private static IEnumerable<Node> GetConditionalEventNodes (
            ApplicationContext context,
            Node args)
        {
            // Checking if we have retrieved operators, and if not, retrieving them
            if (_operators == null) {
            
                // Retrieving all comparison operators and logical operators in system
                _operators = context.RaiseNative ("operators");

                // Then adding all logical operators
                _operators.Add ("or");
                _operators.Add ("xor");
                _operators.Add ("and");
                _operators.Add ("not");
            }

            // Checking if value of args is null, and if so, we use the first child of it as
            // an "active event" operator, which simply will be checked for existence
            Node idxOperator = args.FirstChild;
            if (args.Value == null) {

                // Basic syntax checking
                if (args.Count == 0)
                    throw new LambdaException ("Nothing to conditionally use for branching in conditional statement", args, context);

                // Retrieving first child as Active Event operator, but making sure we do NOT return "" events!
                yield return args.Children.First (ix => ix.Name != "");

                // Incrementing our idxOperator node
                idxOperator = idxOperator.NextSibling;
            }

            // Then returning operators, until we find something that is NOT in our list of comparison/logical operators
            while (idxOperator != null) {

                // Checking if currently iterated node's name is in list of operators
                if (_operators.Children.Count (ix => ix.Name == idxOperator.Name) == 0)
                    yield break;

                // This is a comparison/logical operator
                yield return idxOperator;

                // Incrementing currently iterated node
                idxOperator = idxOperator.NextSibling;
            }
        }
    }
}
