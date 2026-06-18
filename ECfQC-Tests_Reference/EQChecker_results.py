from qiskit import QuantumCircuit
from qiskit.quantum_info import Operator
from qiskit.quantum_info import Statevector
import numpy as np

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

def pretty_format_vector(sv):
    s = "\tnew Complex[] {"
    for a in sv:
        if (a.imag == 0):
            s += str(a.real)+","
        else:
            s += "new("+str(a.real)+"," + str(a.imag)+ "),"
    s+= "}"
    return s

def pretty_format_vector_all(qc_list):
    s = "[\n"
    for b in qc_list:
        s += pretty_format_vector(b)+ ",\n"
    s += "];"
    return s

def simulate(i: int, qc:QuantumCircuit):
    basis_state = np.zeros(2**qc.num_qubits, dtype=complex)
    basis_state[i] = 1.0
    return Statevector(basis_state).evolve(qc)

import numpy as np

def compare_operator_data(d1, d2, delta=1e-11):
    if d1.shape != d2.shape:
        return False

    for i in range(d1.shape[0]):
        for j in range(d1.shape[1]):
            a = d1[i, j]
            b = d2[i, j]

            real_close = abs(a.real - b.real) < delta
            imag_close = abs(a.imag - b.imag) < delta

            if not (real_close and imag_close):
                return False

    return True


qc_list = [
    QuantumCircuit(2),
    QuantumCircuit(2),

    QuantumCircuit(3),
    QuantumCircuit(4),
    QuantumCircuit(4),
    QuantumCircuit(4),

    QuantumCircuit(3),
    QuantumCircuit(3),
    QuantumCircuit(4),
    QuantumCircuit(4),
    QuantumCircuit(4),
    
    QuantumCircuit(4),
    QuantumCircuit(4),
    QuantumCircuit(4),
    
    QuantumCircuit(4),
    QuantumCircuit(4),
    QuantumCircuit(4),
    QuantumCircuit(4),
    QuantumCircuit(4),

    QuantumCircuit(1),
    QuantumCircuit(1),
    QuantumCircuit(1),
    QuantumCircuit(1),
    
    QuantumCircuit(1),
    QuantumCircuit(1),

    QuantumCircuit(3),
    QuantumCircuit(3),
]

qc_list[0].id(0)

qc_list[1].id(0)

qc_list[2].h(0)
qc_list[2].x(1)
qc_list[2].z(0)
qc_list[2].t(0)
qc_list[2].s(0)

qc_list[3].x(3)
qc_list[3].x(3)
qc_list[3].z(2)
qc_list[3].h(2)
qc_list[3].h(0)
qc_list[3].h(1)
qc_list[3].t(0)
qc_list[3].y(1)

qc_list[4].x(3)
qc_list[4].x(3)
qc_list[4].z(2)
qc_list[4].h(2)
qc_list[4].h(0)
qc_list[4].h(1)
qc_list[4].t(0)
qc_list[4].y(1)
qc_list[4].s(1)
qc_list[4].t(1)

qc_list[5].x(3)
qc_list[5].x(3)
qc_list[5].z(2)
qc_list[5].h(2)
qc_list[5].h(0)
qc_list[5].h(1)
qc_list[5].t(0)
qc_list[5].y(1)
qc_list[5].s(1)

qc_list[6].h(0)
qc_list[6].h(1)
qc_list[6].cx(0, 1)
qc_list[6].swap(1,0)

qc_list[7].h(0)
qc_list[7].h(1)
qc_list[7].cx(1, 0)

qc_list[8].h(0)
qc_list[8].h(1)
qc_list[8].cx(0, 1)
qc_list[8].swap(1,0)
qc_list[8].cx(1, 2)
qc_list[8].cx(3, 1)

qc_list[9].h(0)
qc_list[9].h(1)
qc_list[9].cx(0, 1)
qc_list[9].swap(1,0)
qc_list[9].cx(1, 2)
qc_list[9].cx(3, 1)
qc_list[9].swap(0,3)
qc_list[9].cx(3,0)

qc_list[10].h(0)
qc_list[10].h(1)
qc_list[10].cx(0, 1)
qc_list[10].swap(1,0)
qc_list[10].cx(1, 2)
qc_list[10].cx(3, 1)
qc_list[10].swap(0,3)

qc_list[11].h(0)
qc_list[11].h(1)
qc_list[11].h(2)
qc_list[11].h(3)
qc_list[11].ccx(1,2,3)

qc_list[12].h(0)
qc_list[12].h(1)
qc_list[12].h(2)
qc_list[12].h(3)
qc_list[12].ccx(1,2,3)
qc_list[12].ccx(2,3,1)

qc_list[13].h(0)
qc_list[13].h(1)
qc_list[13].h(2)
qc_list[13].h(3)
qc_list[13].ccx(2,3,1)

qc_list[14].h(0)
qc_list[14].h(1)
qc_list[14].x(0)
qc_list[14].y(0)
qc_list[14].ccx(0,1,3)
qc_list[14].cx(1,0)
qc_list[14].swap(0,1)

qc_list[15].h(0)
qc_list[15].h(1)
qc_list[15].x(0)
qc_list[15].y(0)
qc_list[15].ccx(0,1,3)
qc_list[15].cx(0,1)

qc_list[16].h(0)
qc_list[16].h(1)
qc_list[16].x(0)
qc_list[16].y(0)
qc_list[16].cx(0,1)
qc_list[16].y(3)
qc_list[16].t(3)
qc_list[16].cx(2,3)
qc_list[16].ccx(1,2,3)

qc_list[17].h(0)
qc_list[17].h(1)
qc_list[17].x(0)
qc_list[17].y(0)
qc_list[17].cx(0,1)
qc_list[17].y(3)
qc_list[17].t(3)
qc_list[17].cx(2,3)
qc_list[17].s(2)
qc_list[17].swap(1,0)
qc_list[17].ccx(1,2,3)
qc_list[17].ccx(0,3,2)
qc_list[17].y(2)

qc_list[18].h(0)
qc_list[18].h(1)
qc_list[18].x(0)
qc_list[18].y(0)
qc_list[18].cx(0,1)
qc_list[18].y(3)
qc_list[18].t(3)
qc_list[18].cx(2,3)
qc_list[18].s(2)
qc_list[18].swap(1,0)
qc_list[18].ccx(1,2,3)

qc_list[19].x(0)
qc_list[19].y(0)

qc_list[20].z(0)

qc_list[21].x(0)
qc_list[21].y(0)
qc_list[21].z(0)

qc_list[22].id(0)

qc_list[23].h(0)
qc_list[23].x(0)
qc_list[23].h(0)

qc_list[24].s(0)
qc_list[24].s(0)

qc_list[25].cx(0,1)
qc_list[25].cx(1,0)
qc_list[25].cx(0,1)
qc_list[25].id(2)

qc_list[26].swap(0,1)
qc_list[26].h(2)
qc_list[26].h(2)




equivalences = [
    (0,1),
    (2,2),
    (3,3),
    (3,4),
    (3,5),

    (6,6),
    (6,7),
    (8,9),
    (8,10),

    (11,11),
    (11,13),
    (12,13),

    (14,14),
    (14,15),
    (14, 16),
    (14, 18),
    (16, 17),
    (16, 18),

    (10, 18),
    (11, 3),
    (5, 12),
    (2, 6),

    (19,20),
    (20, 21),
    (21, 22),
    (20, 23),
    (20,24),
    (25, 26),
]

result = []
delta = 1e-11
for i in equivalences:
    op1 = Operator(qc_list[i[0]])
    op2 = Operator(qc_list[i[1]])
    data1 = op1.data
    data2 = op2.data
    if compare_operator_data(data1, data2, delta=1e-10):
        result.append(1)
    elif (op1.equiv(op2)):
        result.append(2)
    else:
        result.append(0)

print(result)
