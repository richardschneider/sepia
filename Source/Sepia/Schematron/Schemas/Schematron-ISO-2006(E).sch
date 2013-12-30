<?xml version="1.0" encoding="utf-8" ?>
<!--
(c) International Organization for Standardization 2005.
Permission to copy in any form is granted for use with conforming
SGML systems and applications as defined in ISO 8879,
provided this notice is included in all copies.
-->
<sch:schema xmlns:sch="http://purl.oclc.org/dsdl/schematron" xml:lang="en" defaultPhase="standard">
  <sch:title>Schema for Additional Constraints in Schematron</sch:title>
  <sch:ns prefix="sch" uri="http://purl.oclc.org/dsdl/schematron" />
  <sch:p>
    This schema supplies some constraints in addition to those
    given in the ISO/IEC 19757-2 (RELAX NG Compact Syntax) Schema for Schematron.
  </sch:p>
  <sch:p>
    Richard Schneider added the minimal phase, as specified in section 6.2 of the specification.
    This is basically the compiled schemeatron.
  </sch:p>
  
  <sch:phase id="standard">
    <sch:p>
      The standard schema for a schematron document.
    </sch:p>
    <sch:active pattern="standard-syntax"/>
  </sch:phase>

  <sch:phase id="minimal">
    <sch:p>
      The minimal schema for a compiled schematron document.
    </sch:p>
    <sch:active pattern="minimal-syntax"/>
    <sch:active pattern="standard-syntax"/>
  </sch:phase>

  <sch:pattern id="minimal-syntax">
    <sch:title>ISO/IEC 19757-3:2006(E) Section 6.2 (Minimal)</sch:title>

    <sch:rule context="sch:include">
      <sch:report test="true()">
        <sch:name/> is not allowed in the minimal syntax.
      </sch:report>
    </sch:rule>

    <sch:rule context="sch:pattern[@abstract='true']">
      <sch:report test="true()">
        The abstract pattern <sch:name/> is not allowed in the minimal syntax.
      </sch:report>
    </sch:rule>

    <sch:rule context="sch:pattern[@is-a]">
      <sch:report test="true()">
        The is-a <sch:name/> is not allowed in the minimal syntax.
      </sch:report>
    </sch:rule>

    <sch:rule context="sch:rule[@abstract='true']">
      <sch:report test="true()">
        The abstract rule <sch:name/> is not allowed in the minimal syntax.
      </sch:report>
    </sch:rule>

    <sch:rule context="sch:extends">
      <sch:report test="true()">
        <sch:name/> is not allowed in the minimal syntax.
      </sch:report>
    </sch:rule>

    <sch:rule context="sch:p">
      <sch:report test="true()">
        <sch:name/> is not allowed in the minimal syntax.
      </sch:report>
    </sch:rule>

    <!-- We allow diagnostics and title for logging purposes
    
    <sch:rule context="sch:diagnostics">
      <sch:report test="true()">
        <sch:name/> is not allowed in the minimal syntax.
      </sch:report>
    </sch:rule>

    <sch:rule context="sch:title">
      <sch:report test="true()">
        <sch:name/> is not allowed in the minimal syntax.
      </sch:report>
    </sch:rule>
    -->
  </sch:pattern>
  
  <sch:pattern id="standard-syntax">
    <sch:title>ISO/IEC 19757-3:2006(E)</sch:title>
    <sch:rule context="sch:active">
      <sch:assert
      test="//sch:pattern[@id=current()/@pattern]">
        The pattern attribute of the active element shall match the
        id attribute of a pattern.
      </sch:assert>
    </sch:rule>
    <sch:rule context="sch:pattern[@is-a]">
      <sch:assert
      test="//sch:pattern[@abstract='true'][@id=current()/@is-a]">
        The is-a attribute of a pattern element shall match
        the id attribute of an abstract pattern.
      </sch:assert>
    </sch:rule>
    <sch:rule context="sch:extends">
      <sch:assert
      test="//sch:rule[@abstract='true'][@id=current()/@rule]">
        The rule attribute of an extends element shall match
        the id attribute of an abstract rule.
      </sch:assert>
    </sch:rule>
    <sch:rule context="sch:let">
      <sch:assert
      test = "not(//sch:pattern[@abstract='true']/sch:param[@name=current()/@name])">
        A variable name and an abstract pattern parameter should not
        use the same name.
      </sch:assert>
    </sch:rule>
  </sch:pattern>
</sch:schema>