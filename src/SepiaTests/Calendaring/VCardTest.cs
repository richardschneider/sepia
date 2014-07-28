using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Sepia.Calendaring.Serialization;

namespace Sepia.Calendaring
{

    /// <summary>
    ///   Unit tests for <see cref="VCard"/>.
    /// </summary>
    [TestClass]
    public class VCardTest
    {
        const string Crlf = "\r\n";
        static readonly string Simple =
            "BEGIN:VCARD" + Crlf +
            "PRODID:TEST" + Crlf +
            "VERSION:4.0" + Crlf +
            "END:VCARD" + Crlf;

        /// <summary>
        ///   The default values for properties are defined when an instance is created.
        /// </summary>
        [TestMethod]
        public void Defaults()
        {
            var card = new VCard();
            Assert.AreEqual("Sepia.Calendaring", card.ProductId);
            Assert.AreEqual("4.0", card.Version);
        }

        [TestMethod]
        public void Reading()
        {
            var card = new VCard();
            card.ReadIcs(IcsReader.Create(new StringReader(Simple)));
            Assert.AreEqual("TEST", card.ProductId);
            Assert.AreEqual("4.0", card.Version);
        }

        [TestMethod]
        public void RoundTrip()
        {
            var card = new VCard();
            card.ReadIcs(IcsReader.Create(new StringReader(Simple)));
            var ics2 = new StringWriter();
            card.WriteIcs(IcsWriter.Create(ics2));

            card = new VCard();
            card.ReadIcs(IcsReader.Create(new StringReader(ics2.ToString())));
            Assert.AreEqual("TEST", card.ProductId);
            Assert.AreEqual("4.0", card.Version);
        }

        [TestMethod]
        public void Kind()
        {
            var card = new VCard { Kind = "org" };
            card = WriteAndRead(card);
            Assert.AreEqual("org", card.Kind);
        }

        [TestMethod]
        public void Gender()
        {
            var card = new VCard { Gender = "F" };
            card = WriteAndRead(card);
            Assert.AreEqual("F", card.Gender);
        }

        [TestMethod]
        public void Sources()
        {
            var card = new VCard { Sources = { "http://somewhere.org/me.vcf" } };
            card = WriteAndRead(card);
            Assert.AreEqual("http://somewhere.org/me.vcf", card.Sources[0].Value);
        }

        [TestMethod]
        public void FormattedNames()
        {
            var card = new VCard { FormattedNames = { "Mr. John Q. Public, Esq." } };
            card = WriteAndRead(card);
            Assert.AreEqual("Mr. John Q. Public, Esq.", card.FormattedNames[0].Value);
        }

        [TestMethod]
        public void Names()
        {
            var card = new VCard 
            {
                Names = 
                {
                    new VCardName
                    {
                        FamilyNames = { "Stevenson" },
                        GivenNames = { "John" },
                        AdditionalNames = { "Philip", "Paul" },
                        Prefixes = { "Dr." },
                        Suffixes = { "Jr.", "M.D.", "A.C.P." },
                    }
                }
            };
            card = WriteAndRead(card);
            Assert.AreEqual(1, card.Names.Count);
            Assert.AreEqual("Stevenson", card.Names[0].FamilyNames[0]);
            Assert.AreEqual("John", card.Names[0].GivenNames[0]);
            Assert.AreEqual("Philip", card.Names[0].AdditionalNames[0]);
            Assert.AreEqual("Paul", card.Names[0].AdditionalNames[1]);
            Assert.AreEqual("Dr.", card.Names[0].Prefixes[0]);
            Assert.AreEqual("Jr.", card.Names[0].Suffixes[0]);
            Assert.AreEqual("M.D.", card.Names[0].Suffixes[1]);
            Assert.AreEqual("A.C.P.", card.Names[0].Suffixes[2]);
        }

        [TestMethod]
        public void NickNames()
        {
            var card = new VCard { NickNames = { "Boss" } };
            card = WriteAndRead(card);
            Assert.AreEqual("Boss", card.NickNames[0].Value);
        }

        [TestMethod]
        public void Photos()
        {
            var card = new VCard { Photos = { "http://www.example.com/pub/photos/jqpublic.gif" } };
            card = WriteAndRead(card);
            Assert.AreEqual("http://www.example.com/pub/photos/jqpublic.gif", card.Photos[0].Value);
        }

        [TestMethod]
        public void BirthDate()
        {
            var card = new VCard { BirthDate = new VCardDate { Value = "19960415" } };
            card = WriteAndRead(card);
            Assert.AreEqual("19960415", card.BirthDate.Value);
        }

        [TestMethod]
        public void Address()
        {
            var card = new VCard 
            { 
                Addresses = 
                {
                    new VCardAddress
                    {
                        Label = "line 1\nline 2\r\nline 3",
                        GeographicPositionUrl = "geo:12.3457,78.910",
                        StreetAddress = "123 Main Street",
                        Locality = "Any Town",
                        Region = "CA",
                        PostalCode = "91921-1234",
                        Country = "U.S.A."
                    }
                }
            };
            card = WriteAndRead(card);
            Assert.AreEqual("line 1\nline 2\nline 3", card.Addresses[0].Label);
            Assert.AreEqual("geo:12.3457,78.910", card.Addresses[0].GeographicPositionUrl);
            Assert.AreEqual("123 Main Street", card.Addresses[0].StreetAddress);
            Assert.AreEqual("Any Town", card.Addresses[0].Locality);
            Assert.AreEqual("CA", card.Addresses[0].Region);
            Assert.AreEqual("91921-1234", card.Addresses[0].PostalCode);
            Assert.AreEqual("U.S.A.", card.Addresses[0].Country);
        }

        [TestMethod]
        public void Emails()
        {
            var card = new VCard { Emails = { "x@y.org" } };
            card = WriteAndRead(card);
            Assert.AreEqual("x@y.org", card.Emails[0].Value);
        }

        [TestMethod]
        public void Telephones()
        {
            var card = new VCard { Telephones = { "+64 4 5555555" } };
            card = WriteAndRead(card);
            Assert.AreEqual("+64 4 5555555", card.Telephones[0].Value);
        }

        [TestMethod]
        public void Languages()
        {
            var card = new VCard { Languages = { "en", "fr" } };
            card = WriteAndRead(card);
            Assert.AreEqual("en", card.Languages[0].Value);
            Assert.AreEqual("fr", card.Languages[1].Value);
        }

        [TestMethod]
        public void Timezones()
        {
            var card = new VCard { Timezones = { "Wellington/New Zealand", "utc-offset:+1200" } };
            card = WriteAndRead(card);
            Assert.AreEqual("Wellington/New Zealand", card.Timezones[0].Value);
            Assert.AreEqual("utc-offset:+1200", card.Timezones[1].Value);
        }

        [TestMethod]
        public void GeographicPositions()
        {
            var card = new VCard { GeographicPositions = { "geo:37.386013,-122.082932" } };
            card = WriteAndRead(card);
            Assert.AreEqual("geo:37.386013,-122.082932", card.GeographicPositions[0].Value);
        }

        [TestMethod]
        public void Urls()
        {
            var card = new VCard { Urls = { "https://somewhere.org/me.vcf" } };
            card = WriteAndRead(card);
            Assert.AreEqual("https://somewhere.org/me.vcf", card.Urls[0].Value);
        }

        VCard WriteAndRead(VCard card)
        {
            var ics1 = new StringWriter();
            card.WriteIcs(IcsWriter.Create(ics1));

            card = new VCard();
            card.ReadIcs(IcsReader.Create(new StringReader(ics1.ToString())));
            return card;
        }
    }
}
