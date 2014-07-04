<?xml version="1.0" encoding="utf-8" ?>
<schema xmlns="http://purl.oclc.org/dsdl/schematron" queryBinding="exslt" xml:lang="en" schemaVersion="1.0">
  <title>Uses exslt query binding</title>
  <ns prefix="math" uri="http://exslt.org/math"/>
  
  <pattern>
    <title>XSLT math:min test</title>
    <rule context="prices">
      <assert test="math:min(price) &gt; 0">All prices must be greater than zero.</assert>
    </rule>
  </pattern>

</schema>
