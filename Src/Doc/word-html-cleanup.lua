-- Remove Microsoft Word presentation attributes that force Pandoc's GFM
-- writer to emit raw HTML instead of ordinary Markdown images.
local empty_paragraph_marker = "<!-- word-empty-paragraph -->"

function Image(image)
    image.attr = pandoc.Attr()
    return image
end

-- Preserve non-black foreground colors that convey meaning in the format
-- tables, while discarding Word's default black, language, font, class, and
-- other presentation metadata.
function Span(span)
    local style = span.attributes.style or ""
    local color = style:match("[Cc][Oo][Ll][Oo][Rr]%s*:%s*([^;]+)")
    local attributes = {}
    local span_text = pandoc.utils.stringify(span)
        :gsub(" ", "")
        :gsub("　", "")
        :gsub("﻿", "")
        :gsub("%s", "")

    if color and span_text ~= "" then
        color = color:gsub("^%s+", ""):gsub("%s+$", "")
        local normalized_color = color:lower():gsub("%s", "")
        local is_black = normalized_color == "black"
            or normalized_color == "#000"
            or normalized_color == "#000000"
            or normalized_color == "rgb(0,0,0)"
        if not is_black then
            _G.table.insert(attributes, { "style", "color: " .. color })
        end
    end

    if span.identifier ~= "" or #attributes > 0 then
        span.attr = pandoc.Attr(span.identifier, {}, attributes)
        return span
    end
    return span.content
end

local function clean_row(row)
    row.attr = pandoc.Attr()
    for _, cell in ipairs(row.cells) do
        cell.attr = pandoc.Attr()
        local contents = {}
        for _, block in ipairs(cell.contents) do
            local is_empty_marker = block.t == "RawBlock"
                and block.format == "html"
                and block.text == empty_paragraph_marker
            if not is_empty_marker then
                _G.table.insert(contents, block)
            end
        end
        cell.contents = contents
    end
    return row
end

local function row_is_empty(row)
    for _, cell in ipairs(row.cells) do
        local text = pandoc.utils.stringify(cell.contents)
        text = text:gsub("　", ""):gsub("%s", "")
        if text ~= "" then
            return false
        end
    end
    return true
end

local function row_has_spans(row)
    for _, cell in ipairs(row.cells) do
        if cell.row_span > 1 or cell.col_span > 1 then
            return true
        end
    end
    return false
end

local function table_has_spans(tbl)
    for _, row in ipairs(tbl.head.rows) do
        if row_has_spans(row) then
            return true
        end
    end
    for _, body in ipairs(tbl.bodies) do
        for _, row in ipairs(body.head) do
            if row_has_spans(row) then
                return true
            end
        end
        for _, row in ipairs(body.body) do
            if row_has_spans(row) then
                return true
            end
        end
    end
    for _, row in ipairs(tbl.foot.rows) do
        if row_has_spans(row) then
            return true
        end
    end
    return false
end

local function set_semantic_column_widths(tbl)
    if #tbl.head.rows == 0 or #tbl.colspecs < 2 then
        return
    end

    local description_columns = {}
    local sample_columns = {}
    local column_index = 1
    for _, cell in ipairs(tbl.head.rows[1].cells) do
        local heading = pandoc.utils.stringify(cell.contents)
            :gsub("^%s+", "")
            :gsub("%s+$", "")
            :lower()
        if heading == "description" or heading == "描述" then
            for offset = 0, cell.col_span - 1 do
                description_columns[column_index + offset] = true
            end
        elseif heading == "sample data" or heading == "样例数据" then
            for offset = 0, cell.col_span - 1 do
                sample_columns[column_index + offset] = true
            end
        end
        column_index = column_index + cell.col_span
    end

    local description_count = 0
    for _ in pairs(description_columns) do
        description_count = description_count + 1
    end
    local sample_count = 0
    for _ in pairs(sample_columns) do
        sample_count = sample_count + 1
    end
    local wide_column_count = description_count + sample_count
    if wide_column_count == 0
        or wide_column_count == #tbl.colspecs then
        return
    end

    local description_percent =
        description_count > 0 and (sample_count > 0 and 30 or 35) or 0
    local sample_percent =
        sample_count > 0 and (description_count > 0 and 25 or 35) or 0
    local other_count = #tbl.colspecs - wide_column_count
    local other_percent = 100 - description_percent - sample_percent
    local description_seen = 0
    local sample_seen = 0
    local other_seen = 0

    local function allocated_percent(total, count, position)
        local base = math.floor(total / count)
        local remainder = total - base * count
        return base + (position <= remainder and 1 or 0)
    end

    for index, colspec in ipairs(tbl.colspecs) do
        local percent
        if description_columns[index] then
            description_seen = description_seen + 1
            percent = allocated_percent(
                description_percent,
                description_count,
                description_seen
            )
        elseif sample_columns[index] then
            sample_seen = sample_seen + 1
            percent = allocated_percent(
                sample_percent,
                sample_count,
                sample_seen
            )
        else
            other_seen = other_seen + 1
            percent = allocated_percent(
                other_percent,
                other_count,
                other_seen
            )
        end
        colspec[2] = percent / 100 + 0.0000001
        tbl.colspecs[index] = colspec
    end
end

-- Complex tables with merged cells must remain inline HTML in GFM. Strip
-- Word's presentation attributes while retaining row/column spans.
function Table(tbl)
    tbl.attr = pandoc.Attr()

    -- Word generated these tables entirely with <td> elements. Pandoc
    -- consequently creates an empty header and treats the real first row as
    -- data. Promote that row so GFM renders the intended column headings.
    local header_is_empty = #tbl.head.rows == 0
        or (#tbl.head.rows == 1 and row_is_empty(tbl.head.rows[1]))
    if header_is_empty
        and #tbl.bodies > 0
        and #tbl.bodies[1].body > 0 then
        if #tbl.head.rows == 0 then
            _G.table.insert(tbl.head.rows, tbl.bodies[1].body[1])
        else
            tbl.head.rows[1] = tbl.bodies[1].body[1]
        end
        _G.table.remove(tbl.bodies[1].body, 1)
    end

    set_semantic_column_widths(tbl)

    for index, row in ipairs(tbl.head.rows) do
        tbl.head.rows[index] = clean_row(row)
    end
    for _, body in ipairs(tbl.bodies) do
        for index, row in ipairs(body.head) do
            body.head[index] = clean_row(row)
        end
        for index, row in ipairs(body.body) do
            body.body[index] = clean_row(row)
        end
    end
    for index, row in ipairs(tbl.foot.rows) do
        tbl.foot.rows[index] = clean_row(row)
    end

    if table_has_spans(tbl) then
        local writer_options = pandoc.WriterOptions({ wrap_text = "none" })
        local html = pandoc.write(
            pandoc.Pandoc({ tbl }),
            "html",
            writer_options
        )
        return pandoc.RawBlock("html", html)
    end

    return tbl
end

-- Word uses non-breaking and ideographic spaces as visual paragraph padding.
-- Remove a text block only when it contains no actual content. In a table this
-- leaves the cell itself intact, including any row/column span.
local function inline_is_padding(inline)
    if inline.t == "Space"
        or inline.t == "SoftBreak"
        or inline.t == "LineBreak" then
        return true
    end
    if inline.t ~= "Str" then
        return false
    end

    local text = inline.text
        :gsub(" ", "")
        :gsub("　", "")
        :gsub("﻿", "")
        :gsub("%s", "")
    return text == ""
end

local function remove_empty_text_block(block)
    while #block.content > 0 and inline_is_padding(block.content[1]) do
        _G.table.remove(block.content, 1)
    end
    while #block.content > 0
        and inline_is_padding(block.content[#block.content]) do
        _G.table.remove(block.content)
    end
    if #block.content == 0 then
        return pandoc.RawBlock("html", empty_paragraph_marker)
    end
    return block
end

function Para(paragraph)
    return remove_empty_text_block(paragraph)
end

function Plain(plain)
    return remove_empty_text_block(plain)
end

-- Comm2_MBI uses <pre> around prose notes rather than preformatted data.
-- Convert only that mislabeled section to ordinary paragraphs; genuine code
-- and binary-layout examples in other documents remain code blocks.
function CodeBlock(code_block)
    if not code_block.text:match("^%s*特殊注释") then
        return nil
    end

    local paragraphs = {}
    for line in code_block.text:gmatch("[^\r\n]+") do
        line = line:gsub("^%s+", ""):gsub("%s+$", "")
        if line ~= "" then
            local inlines = {}
            for word in line:gmatch("%S+") do
                if #inlines > 0 then
                    _G.table.insert(inlines, pandoc.Space())
                end
                _G.table.insert(inlines, pandoc.Str(word))
            end
            _G.table.insert(paragraphs, pandoc.Para(inlines))
        end
    end
    return paragraphs
end

-- Markdown's ordinary separator line does not represent a visible empty
-- paragraph. Preserve every top-level empty paragraph from the Word source,
-- whether it separates text, tables, lists, or section labels.
function Pandoc(document)
    local blocks = {}

    for _, block in ipairs(document.blocks) do
        local is_empty_marker = block.t == "RawBlock"
            and block.format == "html"
            and block.text == empty_paragraph_marker
        if is_empty_marker then
            _G.table.insert(
                blocks,
                pandoc.RawBlock("html", "<p>&nbsp;</p>")
            )
        else
            _G.table.insert(blocks, block)
        end
    end

    local grouped_blocks = {}
    local index = 1
    local function normalized_line_text(block)
        return pandoc.utils.stringify(block)
            :gsub(" ", " ")
            :gsub("　", " ")
            :gsub("﻿", "")
            :gsub("^%s+", "")
            :gsub("%s+$", "")
    end

    while index <= #blocks do
        local block = blocks[index]
        local text = block.t == "Para"
            and normalized_line_text(block) or ""

        if block.t == "Para"
            and text:match("Function%s+[%w_]+%s*%(") then
            local code_lines = {}
            local group_index = index
            local found_function_end = false

            while group_index <= #blocks
                and (blocks[group_index].t == "Para"
                    or (blocks[group_index].t == "RawBlock"
                        and blocks[group_index].format == "html"
                        and blocks[group_index].text == "<p>&nbsp;</p>")) do
                local line = blocks[group_index].t == "Para"
                    and normalized_line_text(blocks[group_index]) or ""
                _G.table.insert(code_lines, line)
                group_index = group_index + 1
                if line:match("End%s+Function") then
                    found_function_end = true
                    break
                end
            end

            if found_function_end then
                _G.table.insert(
                    grouped_blocks,
                    pandoc.CodeBlock(
                        table.concat(code_lines, "\n"),
                        pandoc.Attr("", { "vbnet" })
                    )
                )
                index = group_index
            else
                _G.table.insert(grouped_blocks, block)
                index = index + 1
            end
        elseif block.t == "Para" and text:match("{%s*$") then
            local code_lines = {}
            local depth = 0
            local group_index = index

            while group_index <= #blocks
                and blocks[group_index].t == "Para" do
                local line = pandoc.utils.stringify(blocks[group_index])
                local opening_count =
                    select(2, line:gsub("{", ""))
                local closing_count =
                    select(2, line:gsub("}", ""))

                if line:match("^%s*}") then
                    depth = math.max(0, depth - closing_count)
                    closing_count = 0
                end

                _G.table.insert(
                    code_lines,
                    string.rep("    ", depth) .. line
                )
                depth = depth + opening_count - closing_count
                group_index = group_index + 1

                if depth == 0 then
                    break
                end
            end

            if depth == 0 and #code_lines > 1 then
                _G.table.insert(
                    grouped_blocks,
                    pandoc.CodeBlock(table.concat(code_lines, "\n"))
                )
                index = group_index
            else
                _G.table.insert(grouped_blocks, block)
                index = index + 1
            end
        else
            _G.table.insert(grouped_blocks, block)
            index = index + 1
        end
    end

    local function is_vbnet_code_block(block)
        if block.t ~= "CodeBlock" then
            return false
        end
        for _, class in ipairs(block.classes) do
            if class == "vbnet" then
                return true
            end
        end
        return false
    end

    local function format_vb_code(code)
        local lines = {}
        local depth = 0
        local case_active = false

        for line in (code .. "\n"):gmatch("(.-)\r?\n") do
            line = line:gsub("^%s+", ""):gsub("%s+$", "")
            local lower = line:lower()

            if lower:match("^end%s+select")
                and case_active then
                depth = math.max(0, depth - 1)
                case_active = false
            end
            if lower:match("^end%s+")
                or lower:match("^next%f[%A]")
                or lower:match("^loop%f[%A]") then
                depth = math.max(0, depth - 1)
            elseif lower:match("^else")
                or lower:match("^case%f[%A]") then
                if lower:match("^case%f[%A]") and case_active then
                    depth = math.max(0, depth - 1)
                elseif lower:match("^else") then
                    depth = math.max(0, depth - 1)
                end
            end

            _G.table.insert(
                lines,
                line == "" and "" or string.rep("    ", depth) .. line
            )

            if lower:match("^function%f[%A]")
                or lower:match("^for%f[%A]")
                or lower:match("^while%f[%A]")
                or lower:match("^do%s*$")
                or lower:match("^with%f[%A]")
                or lower:match("^if%s+.*%s+then%s*$")
                or lower:match("^else")
                or lower:match("^case%f[%A]") then
                depth = depth + 1
            elseif lower:match("^select%s+case%f[%A]") then
                depth = depth + 1
                case_active = false
            end
            if lower:match("^case%f[%A]") then
                case_active = true
            end
        end

        while #lines > 0 and lines[#lines] == "" do
            _G.table.remove(lines)
        end
        return table.concat(lines, "\n")
    end

    local merged_blocks = {}
    index = 1
    while index <= #grouped_blocks do
        local block = grouped_blocks[index]
        if is_vbnet_code_block(block) then
            local code_parts = { block.text }
            local next_index = index + 1

            while next_index + 1 <= #grouped_blocks
                and grouped_blocks[next_index].t == "RawBlock"
                and grouped_blocks[next_index].format == "html"
                and grouped_blocks[next_index].text == "<p>&nbsp;</p>"
                and is_vbnet_code_block(grouped_blocks[next_index + 1]) do
                _G.table.insert(code_parts, grouped_blocks[next_index + 1].text)
                next_index = next_index + 2
            end

            _G.table.insert(
                merged_blocks,
                pandoc.CodeBlock(
                    format_vb_code(table.concat(code_parts, "\n\n")),
                    pandoc.Attr("", { "vbnet" })
                )
            )
            index = next_index
        else
            _G.table.insert(merged_blocks, block)
            index = index + 1
        end
    end

    local obj_heading_levels = {
        ["Procedures to import and export OBJ"] = 1,
        ["How to convert MBI to OBJ"] = 2,
        ["How to import OBJ to 3DS Max 8.0"] = 2,
        ["How to import OBJ to Polytrans 4.2.1"] = 2,
        ["How to export OBJ from 3DS Max 8.0 for MBI"] = 2,
        ["How to export OBJ from 3DS Max 8.0 for SEC"] = 2,
        ["OBJ导入导出步骤"] = 1,
        ["如何将MBI文件导出为OBJ文件"] = 2,
        ["如何将OBJ文件导入3DS Max 8.0"] = 2,
        ["如何将OBJ文件导入Polytrans 4.2.1"] = 2,
        ["如何从3DS Max 8.0导出OBJ文件用于生成MBI文件"] = 2,
        ["如何从3DS Max 8.0导出OBJ文件用于生成SEC文件"] = 2,
    }

    local function canonical_heading_text(block)
        return pandoc.utils.stringify(block)
            :gsub(" ", " ")
            :gsub("　", " ")
            :gsub("%s+", " ")
            :gsub("^%s+", "")
            :gsub("%s+$", "")
    end

    local first_text = #merged_blocks > 0
        and canonical_heading_text(merged_blocks[1]) or ""
    local is_obj_procedure =
        obj_heading_levels[first_text] == 1

    if is_obj_procedure then
        local obj_blocks = {}
        for _, block in ipairs(merged_blocks) do
            local is_empty_paragraph = block.t == "RawBlock"
                and block.format == "html"
                and block.text == "<p>&nbsp;</p>"
            if not is_empty_paragraph then
                local block_text = canonical_heading_text(block)
                local heading_level = obj_heading_levels[block_text]
                if block.t == "Para" and heading_level then
                    local heading_inlines = {}
                    for word in block_text:gmatch("%S+") do
                        if #heading_inlines > 0 then
                            _G.table.insert(
                                heading_inlines,
                                pandoc.Space()
                            )
                        end
                        _G.table.insert(
                            heading_inlines,
                            pandoc.Str(word)
                        )
                    end
                    _G.table.insert(
                        obj_blocks,
                        pandoc.Header(heading_level, heading_inlines)
                    )
                else
                    _G.table.insert(obj_blocks, block)
                end
            end
        end
        merged_blocks = obj_blocks
    end

    document.blocks = merged_blocks
    return document
end
