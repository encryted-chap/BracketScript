function getTokens(line)
    local output = [[]] -- Tokens output
    
	function numChar(str, c)
		local count = 0;
		for i = 1,#str do
			if (str:sub(i,i) == c) then
				count = count + 1
			end
		end
		return count;
	end
    function InsertTok(type, val) -- Function to insert token based on input code
    	output = output .. "[" .. type .. ":" .. val .. "]"
    end
    types = {
    	keyword={
    		"pass while if for break continue return global def",	
    		function(v) InsertTok("keyword", v) end
    	},
    	eq_operator={
    		"+-*/<>:!.,=(){}[]|%^&@;",
    		function(v) 
    			for i = 1,#v do
    			    if types.eq_operator[3][v:sub(i,i)] then
    				    InsertTok("eq_operator", types.eq_operator[3][v:sub(i,i)])
    				end
    			end
    		end
    	}
    }
    local newLine = ""

	-- This part takes each token and seperates them by a space so they can
	-- be tokenized easier in the next section
    for word in string.gmatch(line, "%S+") do
		print("LENGTH:", #word, word)
        for i = 1,#word do
			print(i, word:sub(i,i))
            chr = word:sub(i,i)

			-- Check if the character is a special character (non-alphanumeric)
            if types.eq_operator[1]:find(chr, 1, true) then
                if i == #line then
                    newLine = newLine .. chr
                else
					x = " "
                    newLine = newLine .. " " .. chr .. " "
                end
            else -- If it isn't a special character, concat it normally
                newLine = newLine .. chr
            end
        end
		newLine = newLine .. " "
		print("[=]", newLine)
		print()
    end
	local tmpstr = ""
	for word in string.gmatch(newLine, "%S+") do
		tmpstr = tmpstr .. word .. " "
	end
	newLine = tmpstr
	print("[!]", newLine)
	print()

	local words = {}
	for word in string.gmatch(newLine, "%S+") do
		words[#words + 1] = word;
	end
		
	tempLine = ""
	tempstr = ""
	quit = false
	local count = 1
	for word in string.gmatch(newLine, "%S+") do
		if quit == true then
			break
		end
		if word:sub(1,1) == "\"" then
			for i=count,#words do
				if (words[i]:sub(#words[i],1) == "\"") or words[i + 1] == nil then
					tempLine = tempLine .. tempstr .. words[i]
					tempstr = ""
					quit = true
					break
				else
					if types.eq_operator[1]:find(words[i], 1, true) then
						tempstr = tempstr .. words[i] .. "&!"
					else
						if types.eq_operator[1]:find(words[i + 1], 1, true) then
							tempstr = tempstr .. words[i] .. "&!"
						else
							tempstr = tempstr .. words[i] .. "&^"
						end
					end
				end
				
			end
		end
		count = count + 1
	end
	local magic = "().%+-*?[]^$"
	local tmpstr = ""
	withSpaces = tempLine:gsub("&^", " ")
	local laterstr = withSpaces
	for i=1,#withSpaces do
		if magic:find(withSpaces:sub(i, i)) and withSpaces:sub(i,i) ~= "" then
			tmpstr = tmpstr .. "%" .. withSpaces:sub(i,i) 
		else
			tmpstr = tmpstr .. withSpaces:sub(i,i)	
		end
	end
	withSpaces = tmpstr:gsub("&!", " ")
	withoutSpaces = tmpstr:gsub(" ", "&^"):gsub("&!", " ")
	newLine = newLine:gsub(withSpaces, withoutSpaces)
		
	-- Part of the lexer that takes the variable names and other characters as well
	-- as the special characters/operators and turns them into tokens based on their type.
	local icount = 1
	skip = false
    for word in string.gmatch(newLine, "%S+") do
        local count = 0
		Cword = ""
		if skip == true then
			if words[icount + 2] == nil then
				break
			else
				Cword = words[icount + 2]
			end
			skip = false
		else 
			Cword = word;
		end
        for keyword in string.gmatch(types.keyword[1], "%S+") do
            if Cword:find(keyword, 1, true) then
				InsertTok("keyword", Cword)
                count = count + 1
            end
        end
        if count == 0 then
			if (types.eq_operator[1]:find(Cword, 1, true)) then
            	InsertTok("eq_operator", Cword)
			else -- If the token is not a special character/operator
				if numChar(Cword, "\"") >= 2 then
					InsertTok("string", Cword:gsub("&^", " "))
				elseif type(tonumber(Cword)) == "number" then
					if words[icount + 1] == "." and type(tonumber(words[icount + 2])) == "number" then
						InsertTok("float_num", Cword .. words[icount + 1] .. words[icount + 2])
						skip = true
					else
						InsertTok("num", Cword)
					end
				else
					InsertTok("unknown_symbol", Cword)
				end
			end
        end
		icount = icount + 1
    end
	
    return output
end
return getTokens(inputline)