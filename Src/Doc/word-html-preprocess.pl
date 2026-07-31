use strict;
use warnings;
use utf8;

local $/;
my $html = <STDIN>;

# Repair malformed header cells found in hand-authored legacy documents:
# <th>heading</td>. Browsers recover from this, but Pandoc treats the table as
# ordinary flow content and loses the entire table structure.
$html =~ s{(<th\b[^>]*>.*?)</td>}{$1</th>}gis;

# Repair table-cell paragraphs that Word/browser HTML leaves implicitly
# closed: <td><p>value</td>. Pandoc otherwise lets that paragraph consume
# following cells and silently drops columns.
$html =~ s{
    (<p\b[^>]*>(?:(?!</p>|</td>).)*)
    </td>
}{$1</p></td>}gisx;

# Pandoc does not expand the legacy HTML <col span="N"> shorthand and may
# truncate otherwise valid cells to the number of explicit col elements.
$html =~ s{
    <col\b([^>]*)>
}{
    my $attributes = $1;
    my $span = 1;
    if ($attributes =~
        s/\s+span\s*=\s*(?:"(\d+)"|'(\d+)'|(\d+))//i) {
        $span = defined $1 ? $1 : defined $2 ? $2 : $3;
    }
    $attributes =~ s{/\s*$}{};
    join q{}, (("<col$attributes>") x $span)
}geix;

# Pandoc ignores the deprecated HTML <font color="..."> attribute. Convert
# every font element to a span, carrying only its foreground color forward.
# Other font attributes are intentionally discarded.
$html =~ s{
    <font\b([^>]*)>
}{
    my $attributes = $1;
    my $color;
    if ($attributes =~
        /\bcolor\s*=\s*(?:"([^"]+)"|'([^']+)'|([^\s>]+))/i) {
        $color = defined $1 ? $1 : defined $2 ? $2 : $3;
    }
    defined $color
        ? qq{<span style="color: $color">}
        : q{<span>}
}geix;
$html =~ s{</font\s*>}{</span>}gi;

print $html;
