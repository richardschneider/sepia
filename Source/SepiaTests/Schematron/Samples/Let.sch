<?xml version="1.0" encoding="utf-8" ?>
<schema xmlns="http://purl.oclc.org/dsdl/schematron" xml:lang="en" >
  <title>Test for rules with a let</title>

  <let name="minSec" value="0"/>
  <let name="maxSec" value="59"/>

  <phase id="no-seconds">
    <let name="minSec" value="0"/>
    <let name="maxSec" value="0"/>
    <active pattern="times"/>
  </phase>

  <pattern id="times">
    <title>Times</title>
    <rule context="time">
      <let name="hour" value="number(substring(.,1,2))"/>
      <let name="minute" value="number(substring(.,4,2))"/>
      <let name="second" value="number(substring(.,7,2))"/>

      <!-- CHECK FOR VALID HH:MM:SS -->
      <assert test="string-length(.)=8 and substring(.,3,1)=':' and substring(.,6,1)=':'">The time element should contain a time in the format HH:MM:SS.</assert>
      <assert test="$hour>=0 and $hour&lt;=23">The hour (<value-of select="$hour"/>) be a value between 0 and 23.</assert>
      <assert test="$minute>=0 and $minute&lt;=59">
        The minutes (<value-of select="$minute"/>) must be a value between 0 and 59.</assert>
      <assert test="$second>=$minSec and $second&lt;=$maxSec">
        The second (<value-of select="$second"/>) must be a value between <value-of select="$minSec"/> and <value-of select="$maxSec"/>.
      </assert>
    </rule>

  </pattern>


</schema>
