using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RuleEngine {
    [TestClass]
    public class UnitTests {
        [TestMethod]
        public void GreaterThanRule_And_OrderClosedRule_Tests() {
            var order = new Order(id: "123-456-111", value: 50, state: State.Closed, total: 100.0m, paymentMethod: PaymentMethod.CreditCard);

            Rule rule1 = new GreaterThanRule() { Value = 30 };
            Rule rule2 = new OrderClosedRule();

            Assert.AreEqual(rule1.Evaluate(order), true);
            Assert.AreEqual(rule2.Evaluate(order), true);

            order.State = State.Open;
            order.Value = 1;

            Assert.AreEqual(rule1.Evaluate(order), false);
            Assert.AreEqual(rule2.Evaluate(order), false);
        }

        [TestMethod]
        public void AndRule_Tests() {
            var order1 = new Order(id: "123-456-111", value: 50, state: State.Closed, total: 100.0m, paymentMethod: PaymentMethod.CreditCard);
            var order2 = new Order(id: "123-456-222", value: 50, state: State.Open, total: 100.0m, paymentMethod: PaymentMethod.CreditCard);

            Rule rule1 = new GreaterThanRule() { Value = 30 };
            Rule rule2 = new OrderClosedRule();

            var rules = new[] { rule1, rule2 };

            //
            var andRule = new AndRule();
            andRule.AddRules(rules);

            Assert.AreEqual(andRule.Evaluate(order1), true);
            Assert.AreEqual(andRule.Evaluate(order2), false);
        }

        [TestMethod]
        public void OrRule_Tests() {
            var order1 = new Order(id: "123-456-111", value: 10, state: State.Closed, total: 100.0m, paymentMethod: PaymentMethod.CreditCard);
            var order2 = new Order(id: "123-456-222", value: 10, state: State.Open, total: 100.0m, paymentMethod: PaymentMethod.CreditCard);

            Rule rule1 = new GreaterThanRule() { Value = 30 };
            Rule rule2 = new OrderClosedRule();

            var rules = new[] { rule1, rule2 };

            var orRule = new OrRule();
            orRule.AddRules(rules);

            Assert.AreEqual(orRule.Evaluate(order1), true);
            Assert.AreEqual(orRule.Evaluate(order2), false);
        }

        [TestMethod]
        public void CriticalMassRule_Tests() {
            var order1 = new Order(id: "123-456-111", value: 50, state: State.Closed, total: 100.0m, paymentMethod: PaymentMethod.CreditCard);
            var order2 = new Order(id: "123-456-222", value: 10, state: State.Closed, total: 100.0m, paymentMethod: PaymentMethod.CreditCard);
            var order3 = new Order(id: "123-456-333", value: 10, state: State.Open, total: 100.0m, paymentMethod: PaymentMethod.CreditCard);

            Rule rule1 = new GreaterThanRule() { Value = 30 };
            Rule rule2 = new OrderClosedRule();

            var rules = new[] { rule1, rule2 };

            var massRules = new CriticalMassRule(minimunRulesCount: 1);
            massRules.AddRules(rules);

            Assert.AreEqual(massRules.Evaluate(order1), true);
            Assert.AreEqual(massRules.Evaluate(order2), true);
            Assert.AreEqual(massRules.Evaluate(order3), false);

            massRules = new CriticalMassRule(minimunRulesCount: 2);
            massRules.AddRules(rules);

            Assert.AreEqual(massRules.Evaluate(order1), true);
            Assert.AreEqual(massRules.Evaluate(order2), false);
            Assert.AreEqual(massRules.Evaluate(order3), false);
        }
    }
}
