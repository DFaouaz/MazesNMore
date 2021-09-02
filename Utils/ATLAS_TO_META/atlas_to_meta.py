
from os import listdir

def from_atlas_to_meta(input_dir, filename, output_dir):
    file = open(input_dir + filename, "rt")
    lines = file.readlines()
    file.close()

    metas_data = []
    # Hasta que acaben las lineas
    # Nos saltamos la primera linea "\n"
    line_index = 1
    while line_index < len(lines):
        # Sin saltos de linea
        meta_data = []
        aux = lines[line_index]
        while aux != "\n" and line_index < len(lines):
            meta_data += [aux.replace("\n", "")]
            line_index += 1
            if line_index < len(lines):
                aux = lines[line_index]
        metas_data.append(meta_data)
        line_index += 1        

    lines_per_header = 5
    lines_per_sprite = 7

    for i in range(len(metas_data)):
        lines = metas_data[i]
        index = lines_per_header

        file_name = lines[0]
        file_size = lines[1]
        file_format = lines[2]
        file_filter = lines[3]
        file_repeat = lines[4]

        file_height = int(file_size.split(":", 1)[1].split(",")[0])

        output_file = open(output_dir + file_name + ".meta", "a")
        count = 0

        while index + lines_per_sprite < len(lines):
            # READ
            # Name
            name = lines[index]
            # Rotate
            rotate = lines[index + 1]
            # xy
            coords = lines[index + 2]
            # size
            size = lines[index + 3]
            # orig
            orig = lines[index + 4]
            # offset
            offset = lines[index + 5]
            # index
            ind = lines[index + 6]

            # PARSE
            split_name = file_name.split(".")
            width = int(size.split(":")[1].split(",")[0])
            height = int(size.split(":")[1].split(",")[1])
            out_name = split_name[-2] + "_" + str(count)
            x = int(coords.split(":")[1].split(",")[0])
            y = file_height - int(coords.split(":")[1].split(",")[1]) - height

            # WRITE
            output_lines = []
            output_lines.append("  - serializedVersion: 2\n")
            output_lines.append("    name: " + out_name + "\n")
            output_lines.append("    rect:\n")
            output_lines.append("      serializedVersion: 2\n")
            output_lines.append("      x: " + str(x) + "\n")
            output_lines.append("      y: " + str(y) + "\n")
            output_lines.append("      width: " + str(width) + "\n")
            output_lines.append("      height: " + str(height) + "\n")
            output_lines.append("    alignment: 0\n")
            output_lines.append("    pivot: {x: 0.5, y: 0.5}\n")
            output_lines.append("    border: {x: 0, y: 0, z: 0, w: 0}\n")

            output_file.writelines(output_lines)
            index += lines_per_sprite
            count += 1

        output_file.close()






filenames = listdir("./input/")

for name in filenames:
    from_atlas_to_meta("./input/", name, "./output/")