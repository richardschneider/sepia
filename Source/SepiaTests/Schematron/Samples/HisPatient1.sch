<?xml version="1.0" encoding="UTF-8"?>
<schema xmlns="http://purl.oclc.org/dsdl/schematron" queryBinding="xslt">
  <title>Schema for a HIS Patient</title>
  <ns prefix="f" uri="http://hl7.org/fhir"/>

  <pattern>
    <title>Patient</title>
    <rule context="f:Patient">
      <report test="count(f:name) > 1">Only one name is allowed.</report>
      <report test="f:deceasedBoolean[@value = 'true']">A date of death is required, not just a death indication.</report>
    </rule>
  </pattern>

</schema>
