from qiskit import QuantumCircuit
from qiskit.quantum_info import Operator

def pretty_format(qc:QuantumCircuit) -> str:
    s = "new DenseMatrix("+ str(pow(2,qc.num_qubits))+", "+str(pow(2,qc.num_qubits))+", ["
    data = Operator(qc).data.flatten(order='F')
    for a in data:
        if (a.imag == 0):
            s += str(a.real)+","
        else:
            s += "new("+str(a.real)+"," + str(a.imag)+ "),"
    s+= "])"
    return s

def pretty_format_all(qc_list):
    s = "[\n"
    for b in qc_list:
        s += pretty_format(b)+ ",\n"
    s += "];"
    return s

qc_list = [
    QuantumCircuit(2),
    QuantumCircuit(2),
    QuantumCircuit(3),
    QuantumCircuit(2),
    QuantumCircuit(2),
    QuantumCircuit(2),
    QuantumCircuit(3),
    QuantumCircuit(4),
    QuantumCircuit(4),
    QuantumCircuit(4),
]

print("simple")

qc_list[0].x(0)
qc_list[1].y(1)
qc_list[2].z(1)
qc_list[3].h(1)
qc_list[4].s(0)
qc_list[5].t(0)
qc_list[6].cx(0,1)
qc_list[7].swap(1,2)
qc_list[8].ccx(0,1,2)
qc_list[9].ccx(1,2,3)

print(pretty_format_all(qc_list))

qc_list = [
    QuantumCircuit(2),
    QuantumCircuit(3),
    QuantumCircuit(3),
    QuantumCircuit(4),
    QuantumCircuit(4),

    QuantumCircuit(3),
    QuantumCircuit(3),
    QuantumCircuit(3),
    QuantumCircuit(4),
    QuantumCircuit(4),
]

print("complex")

qc_list[0].swap(1,0)
qc_list[1].cx(0,2)
qc_list[2].cx(2, 1)
qc_list[3].swap(3,0)
qc_list[4].cx(0,3)

qc_list[5].ccx(2,1,0)
qc_list[6].ccx(2,0,1)
qc_list[7].ccx(1,2,0)
qc_list[8].ccx(1,0,3)
qc_list[9].ccx(3,1,0)

print(pretty_format_all(qc_list))

