<?xml version="1.0" encoding="utf-8" ?>
<schema xmlns="http://purl.oclc.org/dsdl/schematron" xml:lang="en" >
  <title>Doggie by Rick Jelliffe</title>

  <pattern>
    <title>Doggie</title>
    <rule context="dog" role="animal" id="doggy">
      <assert test="undefine-function(1)">XPath error</assert>
      <assert test="count(ear) = 2" role="internalProperty">A 'dog' element should contain two 'ear' elements.</assert>
      <assert test="bone" role="externalProperty">Every dog needs a bone.</assert>
      <assert test="nose or @exceptional='true'" role="internalProperty" diagnostics="d1 d2">A dog should have a nose.</assert>
    </rule>
  </pattern>

  <diagnostics>
    <diagnostic id="d1" >
      Your dog <value-of select="@petname" /> has no nose.
      How does he smell? Terrible. 
    </diagnostic>
    <diagnostic id="d2" >
      Animals usually come with a full complement
      of legs, ears and noses, etc. However, exceptions are possible.
      If your dog is exceptional, provide an attribute
      <emph>exceptional='true'</emph>
    </diagnostic>
  </diagnostics>

</schema>
