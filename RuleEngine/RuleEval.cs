using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleEngine {
    /// <summary>
    /// Rule, all rules should derive from this and implement the evaluate
    /// method which will check if the order satisfies the condition in the rule
    /// </summary>
    public abstract class Rule {
        public abstract bool Evaluate(Order order);
    }

    /// <summary>
    /// Rules in charge of evaluating multiple rules will derive from this class.
    /// The AddRules functionality is inherited by them and all have access to 
    /// its internal _rules member.
    /// </summary>
    public abstract class OperatorRule : Rule {
        public void AddRules(IEnumerable<Rule> rules) {
            _rules = rules;
        }
        protected IEnumerable<Rule> _rules;
    }

    /// <summary>
    /// "And" rule which returns True if and only if all rules are true
    /// </summary>
    public class AndRule : OperatorRule {
        public override bool Evaluate(Order order) {
            foreach (var rule in _rules) {
                if (!rule.Evaluate(order)) {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// "Or" rule which returns True if any of the rules are true
    /// </summary>
    public class OrRule : OperatorRule {
        public override bool Evaluate(Order order) {
            foreach (var rule in _rules) {
                if (rule.Evaluate(order)) {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// "CriticalMass" rule which returns True if at least the specified number of rules are true
    /// </summary>
    public class CriticalMassRule : OperatorRule {
        public CriticalMassRule(int minimunRulesCount) {
            _minimunRulesCount = minimunRulesCount;
        }

        public override bool Evaluate(Order order) {
            return _rules.Where(r => r.Evaluate(order)).Count() >= _minimunRulesCount;
        }

        private int _minimunRulesCount;
    }

    /// <summary>
    /// Compares the value of the order to a given value
    /// The rule evaluates to true if the item value is greater
    /// </summary>
    public class GreaterThanRule : Rule {
        public int Value;

        public override bool Evaluate(Order order) {
            return (order.Value > Value);
        }
    }

    /// <summary>
    /// Compares the state of the order to closed state
    /// The rule evaluates to true if the order state is closed
    /// </summary>
    public class OrderClosedRule : Rule {
        public override bool Evaluate(Order order) {
            return (order.State == State.Closed);
        }
    }

    /// <summary>
    /// Validates the order's card number.
    /// The rule evalues to true when the number of digists are provided and exacly 16.
    /// </summary>
    public class CardNumberValidRule : Rule {
        public override bool Evaluate(Order order) {
            return !String.IsNullOrWhiteSpace(order.CardNumber)
                && order.CardNumber.All(Char.IsDigit)
                && order.CardNumber.Length == 16;
        }
    }
}

