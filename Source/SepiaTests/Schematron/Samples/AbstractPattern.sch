<?xml version="1.0" encoding="utf-8" ?>
<schema xmlns="http://purl.oclc.org/dsdl/schematron" xml:lang="en" >
  <title>Test for abstract patterns</title>

  <pattern abstract="true" id="requiredAttribute">
    <title>Required Attributes</title>
    <rule context=" $context ">
      <assert test="string-length( $attribute ) &gt; 0">
        The <name/> element is missing a required attribute.
      </assert>
    </rule>
  </pattern>

  <pattern is-a="requiredAttribute" id="foo1">
    <title>id is required</title>
    <param name="context" value="foo" />
    <param name="attribute" value="@id" />
  </pattern>

  <pattern is-a="requiredAttribute" id="foo2">
    <title>bar is required</title>
    <param name="context" value="foo" />
    <param name="attribute" value="@bar" />
  </pattern>

</schema>
