<?xml version="1.0" encoding="UTF-8"?>
<schema xmlns="http://www.ascc.net/xml/schematron">
  <title>Schema for UBL-ApplicationResponse-2.0; ; BRM</title>
  <ns uri="urn:oasis:names:specification:ubl:schema:xsd:ApplicationResponse-2" prefix="p1"/>
  <ns uri="urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2" prefix="p4"/>
  <ns uri="urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2" prefix="p3"/>
  <ns uri="urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2" prefix="p2"/>
  <pattern name="Guidelines">
    <rule context="/p1:ApplicationResponse/p4:DocumentResponse/p4:DocumentReference/p4:Attachment/p3:EmbeddedDocumentBinaryObject">
      <report test=".">
	Element p3:EmbeddedDocumentBinaryObject is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:ReceiverParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:AdditionalStreetName">
      <report test=".">
	Element p3:AdditionalStreetName is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:ReceiverParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:AddressFormatCode">
      <report test=".">
	Element p3:AddressFormatCode is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:ReceiverParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:BuildingName">
      <report test=".">
	Element p3:BuildingName is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:ReceiverParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:BuildingNumber">
      <report test=".">
	Element p3:BuildingNumber is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:ReceiverParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:Department">
      <report test=".">
	Element p3:Department is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:ReceiverParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:ID">
      <report test=".">
	Element p3:ID is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:ReceiverParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:PostalZone">
      <report test=".">
	Element p3:PostalZone is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:ReceiverParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:Postbox">
      <report test=".">
	Element p3:Postbox is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:ReceiverParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:StreetName">
      <report test=".">
	Element p3:StreetName is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:ReceiverParty/p4:PartyLegalEntity/p4:RegistrationAddress/p4:AddressLine">
      <report test=".">
	Element p4:AddressLine is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:ReceiverParty/p4:PartyTaxScheme/p4:TaxScheme/p3:Name">
      <report test=".">
	Element p3:Name is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:ReceiverParty/p4:PartyTaxScheme/p4:TaxScheme/p3:TaxTypeCode">
      <report test=".">
	Element p3:TaxTypeCode is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:SenderParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:AdditionalStreetName">
      <report test=".">
	Element p3:AdditionalStreetName is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:SenderParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:AddressFormatCode">
      <report test=".">
	Element p3:AddressFormatCode is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:SenderParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:BuildingName">
      <report test=".">
	Element p3:BuildingName is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:SenderParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:BuildingNumber">
      <report test=".">
	Element p3:BuildingNumber is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:SenderParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:Department">
      <report test=".">
	Element p3:Department is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:SenderParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:ID">
      <report test=".">
	Element p3:ID is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:SenderParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:PostalZone">
      <report test=".">
	Element p3:PostalZone is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:SenderParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:Postbox">
      <report test=".">
	Element p3:Postbox is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:SenderParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:StreetName">
      <report test=".">
	Element p3:StreetName is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:SenderParty/p4:PartyLegalEntity/p4:RegistrationAddress/p4:AddressLine">
      <report test=".">
	Element p4:AddressLine is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:SenderParty/p4:PartyTaxScheme/p4:TaxScheme/p3:Name">
      <report test=".">
	Element p3:Name is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:SenderParty/p4:PartyTaxScheme/p4:TaxScheme/p3:TaxTypeCode">
      <report test=".">
	Element p3:TaxTypeCode is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:Signature/p4:SignatoryParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:AdditionalStreetName">
      <report test=".">
	Element p3:AdditionalStreetName is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:Signature/p4:SignatoryParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:AddressFormatCode">
      <report test=".">
	Element p3:AddressFormatCode is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:Signature/p4:SignatoryParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:BuildingName">
      <report test=".">
	Element p3:BuildingName is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:Signature/p4:SignatoryParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:BuildingNumber">
      <report test=".">
	Element p3:BuildingNumber is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:Signature/p4:SignatoryParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:Department">
      <report test=".">
	Element p3:Department is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:Signature/p4:SignatoryParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:ID">
      <report test=".">
	Element p3:ID is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:Signature/p4:SignatoryParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:PostalZone">
      <report test=".">
	Element p3:PostalZone is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:Signature/p4:SignatoryParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:Postbox">
      <report test=".">
	Element p3:Postbox is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:Signature/p4:SignatoryParty/p4:PartyLegalEntity/p4:RegistrationAddress/p3:StreetName">
      <report test=".">
	Element p3:StreetName is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:Signature/p4:SignatoryParty/p4:PartyLegalEntity/p4:RegistrationAddress/p4:AddressLine">
      <report test=".">
	Element p4:AddressLine is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:Signature/p4:SignatoryParty/p4:PartyTaxScheme/p4:TaxScheme/p3:Name">
      <report test=".">
	Element p3:Name is marked as not used in the given context.</report>
    </rule>
    <rule context="/p1:ApplicationResponse/p4:Signature/p4:SignatoryParty/p4:PartyTaxScheme/p4:TaxScheme/p3:TaxTypeCode">
      <report test=".">
	Element p3:TaxTypeCode is marked as not used in the given context.</report>
    </rule>
  </pattern>
</schema>
