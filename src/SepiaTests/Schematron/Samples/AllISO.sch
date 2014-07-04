<?xml version="1.0" encoding="utf-8" ?>
<?foo bar="x" ?>
<schema xmlns="http://purl.oclc.org/dsdl/schematron" defaultPhase="all"  fpi="fpi" icon="icon" id="all-infoset" schemaVersion="1.0"  queryBinding="xpath" see="see">
  <!--  -->
  <title xml:space="default">The title is "<dir value="rtl" xml:lang="ar">مفتاح معايير الويب!</dir>" in Arabic.</title>
  <ns prefix="t" uri="urn:testing"/>
  
  <p>some text.</p>
  <p>some more text.</p>
  
  <phase fpi="fpi" id="min" icon="icon">
    <p class="foo" icon="icon">phase for <emph>minimal</emph> doc test.</p>
    <p>all for testing</p>
    <let name="a" value="0"/>
    <let name="b" value="0"/>
    <active pattern="testing">some text</active>
  </phase>

  <phase id="all" fpi="fpi" icon="icon" see="see">
    <p>all</p>
    <let name="a" value="1"/>
    <let name="b" value="2"/>
    <active pattern="testing"/>
    <active pattern="foobar"/>
    <active pattern="namespaces"></active>
  </phase>
  
  <pattern icon="icon" id="testing" see="see">
    <p class="class" icon="icon" id="p1">A test pattern</p>
    <rule context="//dummy" abstract="false" id="dummy" role="role" flag="flag" fpi="fpi" icon="icon" see="see" subject="subject">
      <report test="." icon="icon" id="dummy-rule" diagnostics="diag-1" role="role" subject="subject" flag="flag" fpi="fpi" see="see">dummy not allowed.</report>
    </rule>
    <rule context="foo" id="foo">
      <assert test=". = 'bar'" icon="icon" id="foo-rule" diagnostics="diag-1" role="role" subject="subject" flag="flag" fpi="fpi" see="see">foo must be bar</assert>
    </rule>
  </pattern>

  <pattern id="foobar">
    <title>foobar</title>
    <rule context="foobar">
      <assert test=". = 'foobar'" diagnostics="bad-value">
        The element '<name/>' must be foobar</assert>
    </rule>
  </pattern>

  <pattern id="namespaces">
    <rule context="t:dummy">
      <report test=".">
        The element <name/> is not allowed.
      </report>
    </rule>
    <rule id="rule-1" abstract="true">
      <assert test=". = 'fixed'">
        <name/> must be 'fixed
      </assert>
    </rule>
    <rule id="rule-2" abstract="true">
      <extends rule="rule-1"/>
      <assert test="starts-with(@name, 'x')">
        The attribute 'name', of element <name/>, is required and must start with 'x'
      </assert>
    </rule>
    <rule id="rule-3" abstract="true">
      <extends rule="rule-1"/>
      <extends rule="rule-2"/>
    </rule>
    <rule id="rule-4" context="t:fixed-element">
      <extends rule="rule-2"/>
      <assert test="1=1">always true</assert>
    </rule>
  </pattern>
  
  <diagnostics>
    <diagnostic icon="icon" id="diag-1">diag # 1</diagnostic>
    <diagnostic id="bad-value">'<value-of select="."/>' is not an allowed value.</diagnostic>
  </diagnostics> 
  
  
</schema>