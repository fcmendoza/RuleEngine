# RuleEngine code challenge

Base classes:

```csharp
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
```

And, Or and Critical Mass rules:

```csharp
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
```

Concrete rules implementations:

```csharp
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
```

## Unit tests

Found on `RuleEngine/UnitTests.cs`

```csharp
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

    [TestMethod]
    public void CardNumberValidRule_Tests() {
        var order = new Order(id: "123-456-111", value: 50, state: State.Closed, total: 100.0m, paymentMethod: PaymentMethod.CreditCard);

        Rule rule = new CardNumberValidRule();

        Assert.AreEqual(rule.Evaluate(order), false);

        order.CardNumber = "1234";
        Assert.AreEqual(rule.Evaluate(order), false);

        order.CardNumber = "abcd1234efgh5678";
        Assert.AreEqual(rule.Evaluate(order), false);

        order.CardNumber = "1234567890123456";
        Assert.AreEqual(rule.Evaluate(order), true);
    }
}
```
Output 

![image](https://cloud.githubusercontent.com/assets/904058/12370996/42d820ba-bbda-11e5-959c-27aa58736932.png)

## How you could persist these rules offline

* I would store every rule and it's configuration as XML and saved them to a database table.
* The rules would be bundled into profiles (a profile being a collection of rules).
* Every concrete class derived from `Rule` would have to implement a `Serialize` and `Deserialize` operation. That way every rule knows how to create an instance of itself and how to represent its current state as XML.
* A sample XML for a rule could look like this:

```xml
<Rule>
    <Type>GreaterThan</Type>
	<Configuration>
		<Value>30</Value>
	</Configuration>
</Rule>
```
