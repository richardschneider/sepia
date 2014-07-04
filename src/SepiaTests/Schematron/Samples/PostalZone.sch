<?xml version="1.0" encoding="utf-8" ?>
<schema xmlns="http://purl.oclc.org/dsdl/schematron" xml:lang="en" schemaVersion="1.0">
  <title>Checks the postal zones of an UBL document.</title>
  <ns prefix="cac" uri="urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"/>
  <ns prefix="cbc" uri="urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"/>
  <ns prefix="ext" uri="urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2"/>
  <ns prefix="ccts" uri="urn:un:unece:uncefact:documentation:2"/>
  <ns prefix="qdt" uri="urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2"/>
  <ns prefix="udt" uri="urn:un:unece:uncefact:data:draft:UnqualifiedDataTypesSchemaModule:2"/>
  
  <pattern>
    <title>UBL Postal Zone</title>
    <rule context="cbc:PostalZone[following-sibling::cac:Country/cbc:IdentificationCode = 'NZ']">
      <assert test="string-length(.) = 4">A NZ postal zone is 4 digits.</assert>
    </rule>
    <rule context="cbc:PostalZone[following-sibling::cac:Country/cbc:IdentificationCode = 'US']">
      <assert test="string-length(.) = 5 or (string-length(.) = 10 and substring(.,6,1) = '-')">A US postal zone is 5 digits or 5+4 digits.</assert>
    </rule>
  </pattern>

</schema>
