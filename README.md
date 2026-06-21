# Equivalence-Checking-for-Quantum-Circuits
Equivalence-Checking-for-Quantum-Circuits (ECfQC) is a program made to check the equivalence of two quantum circuits. It accompanies a seminar paper and presentation made for the "Seminar on Integration of Quantum Computing Accelerators with HPC Systems" (IN0014) at the Technical University of Munich (TUM). 

The name of said paper is "An Example Implementation of a Quantum Equivalence Checking Routine in C\#".

The aim of this project, paper and presentation is the implementation of an equivalence checking routine for quantum circuits outlined in the paper "Advanced Equivalence Checking for Quantum Circuits" by Lukas Burgholzer and Robert Wille which can be found [here](https://arxiv.org/abs/2004.08420).

Do achieve this, this project implements Quantum Decision Diagrams (QDDs), a simulator, which attempts to find counterexamples to prove non-equivalence by simulating quantum circuits with randomly chosen basis states as input, and an EQChecker calculating $G \rightarrow \mathbb{I} \leftarrow G'$ which can then prove or disprove the equality or equivalence of two quantum circuits.

## Installation
This project requires .Net 9.0. 

After cloning the project from GitHub, open a terminal and navigate into the topmost folder ("Equivalence-Checking-for-Quantum-Circuits"). To compile the program, navigate into "ECfQC" and run either
```shell
dotnet build
```
or 
```shell
dotnet publish
```
The compiled binary can then be found under "\bin\Debug\net9.0\EDfQC.exe" (if build is used) or "\bin\Release\net9.0\publish\EDfQC.exe" (if publish is used). 
Using the latter is recommended as it copies dependencies and creates a more independent project, but the former should work as well. 
Note that everything in the same folder as the .exe-file is required for it to run, meaning that if the .exe-file is moved, the other files need to be moved into the same folder.

The previous outlined step will also build the test project located under "ECfQC-Tests". If you wish to directly build the test project, it is also possible to navigate in said folder and use the same commands.
Additionally, running
```
dotnet test
```
in the test project will both build and run the tests.

The test project requires the MathNET.Numerics library (https://numerics.mathdotnet.com/), however the above commands should account for that. If they don't, the library can be installed using NuGet (https://www.nuget.org/). The normal project can be built without it.

The Python files in the "ECfQC-Tests_Reference" folder are not a core part of the program, as they are only meant to show how the expected values for some test cases were calculated. They can be run as standalone python files, as long as the requirements listed in the "requirements.txt" (Qiskit (https://www.ibm.com/quantum/qiskit) and its dependencies) are installed.

## Usage
```
./ECfQC.exe [options] (-q <arg> | --qubits <args>) <filePath1> <filePath2>
./ECfQC.exe (-h | --help)
```
ECfQC is meant to be run in the command line with additional arguments. The full list of arguments and a description for them can be found by running
```shell
./ECfQC.exe -h
```
or
```shell
./ECfQC.exe --help
```
or below:

The required argument `-q <arg>` or `--qubits <arg>` denotes the number of qubits that the two given circuits are applied on.

Two additional arguments are required, each being the path to a file containing the description of a quantum circuit. Regarding these files, please refer to the next section. 

Note that if relative paths are given, they will refer to the working directory, i.e. the directory where the compiled binary is located. As this is very deep in the filesystem, using absolute paths is recommended. For an example of using relative paths, refer to "\Equivalence-Checking-for-Quantum-Circuits\ECfQC-Tests\General Tests\Other Tests\ParserTests.cs".

`-b <arg>` allows the user to adjust the number of simulations to run. The simulations are run with randomly chosen basis states using Random.Random.Next(). If `<arg>` is not given, the default is the same as the amount of qubits. If `<arg>` is 0, the simulation step is skipped. Note that it is possible for the program to run a smaller amount of `<arg>` simulations if a distinct random value can't be generated in a certain number of tries.

`-v` or `--verbose` can be set optionally to get more information to be printed on the console. Note that even with this option, the full matrices or decision diagrams are never displayed as it is assumed that they are too large to be properly formattable.

The argument `-s <arg>` or `--strategy <arg>` is used to determine the strategy that will be used to determine $G \rightarrow \mathbb{I} \leftarrow G'$. As the names of the strategies in the referenced paper are not always clear on what they entail, some names were changed. The full list of all available strategies can be found below:

| Strategy-Id | Name in referenced paper | Description
| - | - | - | 
| alternating | naive | alternates between choosing left and right with each application |
| alternating-balanced | proportional | similar to alternating; however, to avoid an imbalance in the case of differently-sized circuits, the switching occurs proportionally to the ratio of the two circuits' relative length to each other |
| look-ahead | look-ahead | calculates both options in each step and chooses the one that results in a smaller QDD representation|

The default strategy is "look-ahead".


### Files
ECfQC requires the quantum circuits it compares to be provided in separate textfiles using their own syntax. Each circuit is seen as a sequential list of quantum gates.

Each line has to be formatted as follows:
```
<gate> [<q1>] [<q2>] [<q3>]
```
Here \<gate\> is the id of the gate that should be applied, while \<q1\>, \<q2\> and \<q3\> denote the qubits, that this gate should be applied on.

The full list of gates available by default and the number of arguments they take are seen in the table below:

| Gate | Id | Matrix | Number of arguments 
| - | - | - | - |
| Identity | id | $`\begin{bmatrix}1&0&\cdots&0 \\0&1&\cdots&0 \\ \vdots&\vdots&\ddots&\vdots  \\0&0&\cdots&1 \end{bmatrix}`$ | 0 | An empty file is interpreted as if it contains and identity gate |
| Pauli-X | pX | $`\begin{bmatrix} 0 & 1 \\ 1 & 0 \end{bmatrix}`$ | 1 | 
| Pauli-Y | pY | $`\begin{bmatrix}0 &-i \\i & 0 \end{bmatrix}`$ | 1 | 
| Pauli-Z | pZ | $`\begin{bmatrix}1 &0 \\0 & -1 \end{bmatrix}`$ | 1 | 
| Hadamard | h | $`\frac{1}{\sqrt{2}}\begin{bmatrix}1 &1 \\1 & 1 \end{bmatrix}`$ | 1 | 
| S gate | s | $`\begin{bmatrix}1 &0 \\0 & i \end{bmatrix}`$ | 1 | 
| T gate | t | $`\begin{bmatrix}1 &0 \\0 & e^{i\frac{\pi}{4}} \end{bmatrix}`$ | 1 | 
| C-Not | cnot | $`\begin{bmatrix}1&0&0&0 \\0&0&0&1 \\ 0&0&1&0 \\0&1&0&0 \end{bmatrix}`$ | 2 | 
| Swap | sw | $`\begin{bmatrix}1&0&0&0 \\0&0&1&0 \\ 0&1&0&0 \\0&0&0&1 \end{bmatrix}`$ | 2 | 
| Toffoli | t | $`\begin{bmatrix}1&0&0&0&0&0&0&0 \\0&1&0&0&0&0&0&0 \\ 0&0&1&0&0&0&0&0 \\0&0&0&0&0&0&0&1\\0&0&0&0&1&0&0&0\\0&0&0&0&0&1&0&0\\0&0&0&0&0&0&1&0\\0&0&0&1&0&0&0&0 \end{bmatrix}`$ | 3 | 

It is possible to add more gates by appending their id together with their gate matrices to DDMatrix.gates (and rebuilding the code). However, only gates with 1-3 arguments are supported and $S\cdot S^H = \mathbb{I}$ has to hold for any gate matrix S. Specifically gates with 0 arguments other than the identity gate are _not_ supported.

If a file is empty, it is interpreted as a circuit which is represented by the identity matrix of the correct size.

