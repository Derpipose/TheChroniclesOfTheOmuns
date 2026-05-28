// using NUnit.Framework.Internal;
// using PlayerApp.Models;

// namespace ChroniclesTest;

// [TestFixture]
// public class SpellLoadingTests {
//     private SpellService spellService;

//     [SetUp]
//     public void Setup() {
//         spellService = new SpellService();
//     }

//     [Test]
//     [Order(1)]
//     public void TestSpellPullWorks() {
//         List<Spell> list = spellService.GetAllSpellsAsync().Result;

//         Assert.That(list.Count, Is.EqualTo(512));
//     }

//     [Test]
//     [Order(2)]
//     public void TestSpellDetailsAreCorrect() {
//         Spell? balance = spellService.GetAllSpellsAsync().Result.Find(s => s.Name == "Balance");
//         Assert.That(balance, Is.Not.Null);
//         Assert.Multiple(() => {
//             Assert.That(balance?.Name, Is.EqualTo("Balance"));
//             Assert.That(balance?.SpellBook, Is.EqualTo("Force"));
//             Assert.That(balance?.BookLevel, Is.EqualTo(1));
//             Assert.That(balance?.SpellBranch, Is.EqualTo("Force"));
//             Assert.That(balance?.Description, Is.EqualTo("Balance and stabilize yourself or your target."));
//             Assert.That(balance?.ManaCost, Is.EqualTo("1"));
//             Assert.That(balance?.Range, Is.EqualTo(null));
//             Assert.That(balance?.HitDiceId, Is.EqualTo(null));
//             Assert.That(balance?.HitDie, Is.EqualTo(null));
//         });
//     }

//     [Test]
//     [Order(3)]
//     public void TestCantripSpellDetailsAreCorrect() {
//         Spell? ambrosia = spellService.GetAllSpellsAsync().Result.Find(s => s.Name == "Ambrosia");
//         Assert.That(ambrosia, Is.Not.Null);
//         Assert.Multiple(() => {
//             Assert.That(ambrosia?.Name, Is.EqualTo("Ambrosia"));
//             Assert.That(ambrosia?.SpellBook, Is.EqualTo("Celestia"));
//             Assert.That(ambrosia?.BookLevel, Is.EqualTo(0));
//             Assert.That(ambrosia?.SpellBranch, Is.EqualTo("Cantrips"));
//             Assert.That(ambrosia?.Description, Is.EqualTo("Small prayer beads that deal 1D6. Can \"bless\" water to be holy water."));
//             Assert.That(ambrosia?.ManaCost, Is.EqualTo("0"));
//             Assert.That(ambrosia?.Range, Is.EqualTo(null));
//             Assert.That(ambrosia?.HitDiceId, Is.EqualTo(null));
//             Assert.That(ambrosia?.HitDie, Is.EqualTo(null));
//         });
//     }

// }
