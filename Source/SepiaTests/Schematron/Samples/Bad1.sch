<?xml version="1.0" encoding="utf-8" ?>
<schema xmlns="http://www.ascc.net/xml/schematron">
  <title xml:space="default">The title is "<dir value="rtl" xml:lang="ar">مفتاح معايير الويب!</dir>" in Arabic.</title>
  <pattern name="Testing">
    <!-- The x-rule element is undefined; it should be named rule-->
    <x-rule context="dummy">
      <report test=".">dummy not allowed.</report>
    </x-rule>
  </pattern>
</schema>