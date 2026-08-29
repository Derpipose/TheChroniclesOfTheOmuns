export interface DiceResult {
  notation: string;
  rolls: number[];
  rollSum: number;
  modifier: number;
  total: number;
}

export interface Token {
  type: "dice" | "number" | "operator";
  value: string;
}

export function tokenize(input: string): Token[] {
  // Regex breakdown:
  // (\d*)d((\d*)d\d+|\d+) - dice notation with optional nesting (1d4d8 or 1d20)
  // \d+ - plain numbers
  // [*+] - operators
  const tokenStrings = input.match(/(\d*)d((\d*)d\d+|\d+)|\d+|[*+]/gi);

  if (!tokenStrings) {
    return [];
  }

  return tokenStrings.map((token) => {
    if (/^[*+]$/.test(token)) {
      return { type: "operator", value: token };
    } else if (/^(\d*)d/i.test(token)) {
      return { type: "dice", value: token };
    } else {
      return { type: "number", value: token };
    }
  });
}

export function getMultiplyChains(tokens: Token[]): string[][] {
  const chains: string[][] = [];
  let currentChain: string[] = [];

  for (let i = 0; i < tokens.length; i++) {
    const token = tokens[i];

    if (token.type === "dice" || token.type === "number") {
      currentChain.push(token.value);
    } else if (token.type === "operator") {
      if (token.value === "*") {
        // Continue chain, next operand will be added
      } else if (token.value === "+") {
        // Chain ends, save it
        if (currentChain.length > 0) {
          chains.push(currentChain);
          currentChain = [];
        }
      }
    }
  }

  // Push final chain if exists
  if (currentChain.length > 0) {
    chains.push(currentChain);
  }

  return chains;
}

function rollDice(numDice: number, faceCount: number): number[] {
  const rolls: number[] = [];
  for (let i = 0; i < numDice; i++) {
    rolls.push(Math.floor(Math.random() * faceCount) + 1);
  }
  return rolls;
}

function parseSingleDice(notation: string): { numDice: number; faceCount: number } {
  const match = notation.match(/^(\d*)d(\d+)$/i);
  if (!match) {
    throw new Error(`Invalid dice notation: ${notation}`);
  }
  const numDice = match[1] ? parseInt(match[1], 10) : 1;
  const faceCount = parseInt(match[2], 10);
  return { numDice, faceCount };
}

// function resolveMultiplyChain(chain: string[]): string {
//   let multiplier = 0;
//   let finalNotation = "";

//   // Process left-to-right: roll first operand to get multiplier, scale subsequent dice
//   for (let i = 0; i < chain.length; i++) {
//     const operand = chain[i];

//     if (/^(\d*)d/i.test(operand)) {
//       // This is a dice notation
//       const { numDice, faceCount } = parseSingleDice(operand);

//       if (i === 0) {
//         // First operand: roll it to get the multiplier
//         const rolls = rollDice(numDice, faceCount);
//         multiplier = rolls.reduce((a, b) => a + b, 0);
//         finalNotation = operand; // Save in case chain length is 1
//       } else {
//         // Subsequent operand: scale it by the multiplier
//         const scaledDiceCount = multiplier * numDice;
//         finalNotation = `${scaledDiceCount}d${faceCount}`;
//         // If there are more operands, this becomes the multiplier
//         // For now, just update for the final notation
//         multiplier = 1; // Reset since we've built the notation
//       }
//     } else {
//       // This is a plain number
//       const num = parseInt(operand, 10);

//       if (i === 0) {
//         multiplier = num;
//       } else {
//         // Multiply by current multiplier (shouldn't normally happen but handle it)
//         multiplier *= num;
//       }
//     }
//   }

//   return finalNotation;
// }

type EvaluatedFactor = {
  kind: "number" | "dice";
  total: number;
  rolls: number[];
};

function evaluateFactor(input: string): EvaluatedFactor {
  const factor = input.replace(/\s/g, "");
  const nestedMatch = factor.match(/^(\d*)d(\d+)d(\d+)$/i);

  if (nestedMatch) {
    const multiplierRolls = rollDice(
      nestedMatch[1] ? parseInt(nestedMatch[1], 10) : 1,
      parseInt(nestedMatch[2], 10),
    );
    const multiplier = multiplierRolls.reduce((sum, roll) => sum + roll, 0);
    const rolls = rollDice(multiplier, parseInt(nestedMatch[3], 10));

    return {
      kind: "dice",
      rolls,
      total: rolls.reduce((sum, roll) => sum + roll, 0),
    };
  }

  if (/^\d+d\d+$/i.test(factor)) {
    const { numDice, faceCount } = parseSingleDice(factor);
    const rolls = rollDice(numDice, faceCount);

    return {
      kind: "dice",
      rolls,
      total: rolls.reduce((sum, roll) => sum + roll, 0),
    };
  }

  if (/^\d+$/.test(factor)) {
    return { kind: "number", rolls: [], total: parseInt(factor, 10) };
  }

  throw new Error(`Invalid dice expression: ${input}`);
}

export function resolveExpression(
  input: string,
): { resolvedExpr: string; allRolls: number[]; total: number } {
  const normalizedInput = input.replace(/\s/g, "");
  const terms = input.match(/[+-]?[^+-]+/g);

  if (!terms || terms.join("").replace(/\s/g, "") !== normalizedInput) {
    throw new Error("Invalid dice expression");
  }

  const allRolls: number[] = [];
  let total = 0;

  for (const rawTerm of terms) {
    const negative = rawTerm.trim().startsWith("-");
    const term = rawTerm.replace(/^[+-]/, "").trim();
    const factorInputs = term.split("*");
    const factors = factorInputs.map((factorInput, factorIndex) =>
      factorIndex > 0 && /^\s*\d*d\d+\s*$/i.test(factorInputs[factorIndex - 1])
        ? null
        : evaluateFactor(factorInput),
    );

    let termTotal = 1;
    let diceRolls: number[] = [];
    let numericMultiplier = 1;

    for (let factorIndex = 0; factorIndex < factors.length; factorIndex++) {
      const factor = factors[factorIndex];

      if (!factor) {
        continue;
      }

      if (factor.kind === "number") {
        numericMultiplier *= factor.total;
        continue;
      }

      const isFinalDiceFactor = factorIndex === factors.length - 1;
      if (isFinalDiceFactor) {
        diceRolls = factor.rolls.map((roll) => roll * numericMultiplier);
        termTotal *= factor.total * numericMultiplier;
      } else {
        const multiplier = factor.total;
        if (factorIndex + 1 >= factorInputs.length) {
          throw new Error("Dice multipliers must be followed by dice");
        }

        const nextDice = parseSingleDice(factorInputs[factorIndex + 1].trim());
        const scaledRolls = rollDice(multiplier * nextDice.numDice, nextDice.faceCount);
        const scaledTotal = scaledRolls.reduce((sum, roll) => sum + roll, 0);
        diceRolls = scaledRolls.map((roll) => roll * numericMultiplier);
        termTotal *= scaledTotal * numericMultiplier;
        factorIndex++;
      }
    }

    if (factors.filter((factor): factor is EvaluatedFactor => factor !== null)
      .every((factor) => factor.kind === "number")) {
      termTotal = numericMultiplier;
    }

    if (negative) {
      termTotal *= -1;
      diceRolls = diceRolls.map((roll) => roll * -1);
    }

    total += termTotal;
    allRolls.push(...diceRolls);

    if (factors.filter((factor): factor is EvaluatedFactor => factor !== null)
      .every((factor) => factor.kind === "number")) {
      allRolls.push(Math.abs(termTotal));
    }
  }

  return {
    resolvedExpr: terms.map((term) => term.trim()).join(" "),
    allRolls,
    total,
  };
}
// Test: show multiply chains
export function analyzeExpression(input: string): void {
  console.log(`Input: ${input}`);
  const tokens = tokenize(input);
  console.log(`Tokens:`, tokens);
  const chains = getMultiplyChains(tokens);
  console.log(`Multiply chains:`, chains);
  const result = resolveExpression(input);
  console.log(`Resolved:`, result);
}

export function roll(diceInput: string): DiceResult {
  if (!diceInput) {
    throw new Error("Please provide a dice notation (e.g., 3d8 or 3d8+5)");
  }

  const match = diceInput.match(/^(\d*)d((\d*)d\d+|\d+)(\s*[*+]\s*((\d*)d((\d*)d\d+|\d+)|\d+))*$/i);

  if (!match) {
    throw new Error(
      "Invalid dice notation! Use format: XdY, dY, XdY+Z, or dY+Z\n" +
        "Examples: 3d8, d20, d8+5"
    );
  }

  const numDice = match[1] ? parseInt(match[1], 10) : 1;
  const faceCount = parseInt(match[2], 10);
  const modifier = match[3] ? parseInt(match[3], 10) : 0;

  if (numDice <= 0 || faceCount <= 0) {
    throw new Error("Number of dice and face count must be greater than 0");
  }

  if (numDice > 1000) {
    throw new Error("Too many dice! Maximum 1000 dice per roll.");
  }

  const rolls = rollDice(numDice, faceCount);
  const rollSum = rolls.reduce((a, b) => a + b, 0);
  const total = rollSum + modifier;

  return {
    notation: diceInput,
    rolls,
    rollSum,
    modifier,
    total,
  };
}


// function evaluateDiceToken(diceToken: string): number[] {
//   const dCount = (diceToken.match(/d/gi) || []).length;

//   if (dCount === 1) {
//     const { numDice, faceCount } = parseSingleDice(diceToken);
//     return rollDice(numDice, faceCount);
//   }

//   if (dCount > 1) {
//     const firstDIndex = diceToken.search(/d/i);
//     const leftPart = diceToken.slice(0, firstDIndex + 1);
//     const restPart = diceToken.slice(firstDIndex + 1);

//     const nextDIndex = restPart.search(/d/i);
//     const leftFaceStr = restPart.slice(0, nextDIndex);
//     const rightPart = restPart.slice(nextDIndex);

//     const completeLeft = leftPart + leftFaceStr;

//     const { numDice, faceCount } = parseSingleDice(completeLeft);
//     const leftRolls = rollDice(numDice, faceCount);
//     const multiplier = leftRolls.reduce((a, b) => a + b, 0);

//     const newRight = multiplier + rightPart;
//     const rightRolls = evaluateDiceToken(newRight);

//     // Only return the right rolls, not the left
//     return rightRolls;
//   }

//   throw new Error(`Invalid dice token: ${diceToken}`);
// }
